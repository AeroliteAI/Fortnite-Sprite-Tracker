using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FortniteSpriteTracker.Models;
using FortniteSpriteTracker.Services;

namespace FortniteSpriteTracker.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISpriteLoaderService   _spriteLoader;
    private readonly ICollectionDataService _collectionData;
    private readonly ImageCacheService      _imageCache;
    private readonly AppSettingsService     _appSettings;

    private List<SpriteCardViewModel> _allCards = [];
    private CancellationTokenSource?  _saveCts;
    private readonly HashSet<string>  _collapsedGroups = new(StringComparer.OrdinalIgnoreCase);

    private Bitmap? _masteredIcon;
    private Bitmap? _notMasteredIcon;
    private Dictionary<string, Bitmap> _rarityIcons = [];

    public ObservableCollection<SpriteGroupViewModel> DisplayedSprites { get; } = [];
    public ObservableCollection<SpriteCardViewModel>  OverlayCards     { get; } = [];

    public IReadOnlyList<string> SortOptions { get; } =
    [
        "Default (Group Order)",
        "Name A → Z",
        "Name Z → A",
        "Collected First",
        "Not Collected First",
        "Mastered First",
        "Unmastered First",
        "Rarity",
    ];

    [ObservableProperty] private string     _searchText               = string.Empty;
    [ObservableProperty] private FilterMode _activeFilter             = FilterMode.All;
    [ObservableProperty] private int        _selectedSortIndex        = 0;
    [ObservableProperty] private int        _totalCount;
    [ObservableProperty] private int        _collectedCount;
    [ObservableProperty] private int        _masteredCount;
    [ObservableProperty] private double     _collectionProgress;
    [ObservableProperty] private double     _masteryProgress;
    [ObservableProperty] private string     _collectionPercentageText = "0%";
    [ObservableProperty] private string     _masteryPercentageText    = "0%";
    [ObservableProperty] private bool       _isEmpty;
    [ObservableProperty] private bool       _isFilteredEmpty;
    [ObservableProperty] private bool       _isLoading;
    [ObservableProperty] private string     _statusMessage            = string.Empty;
    [ObservableProperty] private int        _cardScalePercent         = 100;
    [ObservableProperty] private GroupMode  _groupMode                = GroupMode.ByVariant;
    [ObservableProperty] private bool       _animateBadges            = true;
    [ObservableProperty] private int        _panelOpacity             = 80;
    [ObservableProperty] private int        _overlayScrollSpeed       = 80;
    [ObservableProperty] private bool       _overlayServerEnabled     = false;
    [ObservableProperty] private string     _overlayOutputPath        = string.Empty;

    public double CardScaleFactor => CardScalePercent / 100.0;

    partial void OnCardScalePercentChanged(int value)
    {
        OnPropertyChanged(nameof(CardScaleFactor));
        SaveSettings();
    }

    partial void OnGroupModeChanged(GroupMode value) { ApplyFilterAndSort(); SaveSettings(); }

    partial void OnPanelOpacityChanged(int value) { ApplyPanelOpacity(value); SaveSettings(); }

    private static void ApplyPanelOpacity(int percent)
    {
        var alpha = (byte)Math.Round(255 * Math.Clamp(percent, 0, 100) / 100.0);
        if (Avalonia.Application.Current?.Resources is { } res)
        {
            res["PanelBrush"]    = new SolidColorBrush(Color.FromArgb(alpha, 0x05, 0x08, 0x0D));
            res["PanelBrushAlt"] = new SolidColorBrush(Color.FromArgb(alpha, 0x0D, 0x16, 0x21));
            res["CardBrush"]     = new SolidColorBrush(Color.FromArgb(alpha, 0x21, 0x38, 0x53));
        }
    }

    partial void OnAnimateBadgesChanged(bool value)   { SpecialBadgeAnimator.SetAnimating(value); SaveSettings(); }
    partial void OnOverlayScrollSpeedChanged(int value) { ApplyFilterAndSort(); SaveSettings(); }

    partial void OnOverlayServerEnabledChanged(bool value)
    {
        if (value) OverlayHttpServer.Start();
        else       OverlayHttpServer.Stop();
        SaveSettings();
    }

    partial void OnOverlayOutputPathChanged(string value)
    {
        _ = OverlayExportService.WriteHtmlAsync(value);
        SaveSettings();
    }

    private void SaveSettings() => _ = _appSettings.SaveAsync(new AppSettings
    {
        CardScalePercent     = CardScalePercent,
        GroupMode            = GroupMode,
        AnimateBadges        = AnimateBadges,
        PanelOpacity         = PanelOpacity,
        OverlayScrollSpeed   = OverlayScrollSpeed,
        OverlayServerEnabled = OverlayServerEnabled,
        OverlayOutputPath    = OverlayOutputPath,
    });

    public MainViewModel(
        ISpriteLoaderService   spriteLoader,
        ICollectionDataService collectionData,
        ImageCacheService      imageCache,
        AppSettingsService     appSettings)
    {
        _spriteLoader   = spriteLoader;
        _collectionData = collectionData;
        _imageCache     = imageCache;
        _appSettings    = appSettings;
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;

        _masteredIcon    = ImageCacheService.LoadAssetImage("Mastered.png");
        _notMasteredIcon = ImageCacheService.LoadAssetImage("NotMastered.png");
        _rarityIcons     = LoadRarityIcons();

        await SpecialBadgeAnimator.InitializeAsync(Path.Combine(AppPaths.Assets, "SPECIAL"));

        var settings         = await _appSettings.LoadAsync();
        CardScalePercent     = settings.CardScalePercent;
        GroupMode            = settings.GroupMode;
        AnimateBadges        = settings.AnimateBadges;
        PanelOpacity         = settings.PanelOpacity;
        ApplyPanelOpacity(PanelOpacity);
        OverlayScrollSpeed   = settings.OverlayScrollSpeed;
        OverlayServerEnabled = settings.OverlayServerEnabled;
        OverlayOutputPath    = settings.OverlayOutputPath;

        await OverlayExportService.WriteHtmlAsync(OverlayOutputPath);

        var spriteModels = await _spriteLoader.LoadSpritesAsync(AppPaths.Sprites);
        var saveData     = await _collectionData.LoadAsync();

        _allCards = await BuildCardsAsync(spriteModels, saveData);

        ApplyFilterAndSort();
        RecalculateStats();

        IsLoading = false;
    }

    // ── Commands ─────────────────────────────────────────────────────────────

    [RelayCommand]
    private async Task RefreshSprites()
    {
        IsLoading    = true;
        var snapshot = BuildSaveSnapshot();
        var models   = await _spriteLoader.LoadSpritesAsync(AppPaths.Sprites);

        foreach (var card in _allCards) { card.StateChanged -= OnCardStateChanged; card.Cleanup(); }

        _allCards = await BuildCardsAsync(models, snapshot);
        ApplyFilterAndSort();
        RecalculateStats();
        IsLoading = false;
    }

    [RelayCommand] private void EnableBadgeAnimation()  => AnimateBadges        = true;
    [RelayCommand] private void DisableBadgeAnimation() => AnimateBadges        = false;
    [RelayCommand] private void EnableOverlayServer()   => OverlayServerEnabled = true;
    [RelayCommand] private void DisableOverlayServer()  => OverlayServerEnabled = false;

    [RelayCommand] private void FilterAll()          => ActiveFilter = FilterMode.All;
    [RelayCommand] private void FilterCollected()    => ActiveFilter = FilterMode.Collected;
    [RelayCommand] private void FilterNotCollected() => ActiveFilter = FilterMode.NotCollected;
    [RelayCommand] private void FilterMastered()     => ActiveFilter = FilterMode.Mastered;
    [RelayCommand] private void FilterNotMastered()  => ActiveFilter = FilterMode.NotMastered;
    [RelayCommand] private void FilterIncomplete()   => ActiveFilter = FilterMode.Incomplete;
    [RelayCommand] private void FilterCompleted()    => ActiveFilter = FilterMode.Completed;
    [RelayCommand] private void ClearSearch()        => SearchText   = string.Empty;

    [RelayCommand] private void GroupByVariant() => GroupMode = GroupMode.ByVariant;
    [RelayCommand] private void GroupByType()    => GroupMode = GroupMode.ByType;

    partial void OnSearchTextChanged(string value)       => ApplyFilterAndSort();
    partial void OnActiveFilterChanged(FilterMode value) => ApplyFilterAndSort();
    partial void OnSelectedSortIndexChanged(int value)   => ApplyFilterAndSort();

    // ── Card building ────────────────────────────────────────────────────────

    private Dictionary<string, Bitmap> LoadRarityIcons()
    {
        var icons = new Dictionary<string, Bitmap>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in new[] { "RARE", "EPIC", "LEGENDARY", "MYTHIC" })
            icons[name] = ImageCacheService.LoadAssetImage($"{name}.png");
        return icons;
    }

    private async Task<List<SpriteCardViewModel>> BuildCardsAsync(
        IReadOnlyList<SpriteModel>     models,
        Dictionary<string, SpriteData> saveData)
    {
        var cards = models.Select(m =>
        {
            saveData.TryGetValue(m.FileName, out var data);

            var rarityIcon = m.Rarity == "SPECIAL"
                ? SpecialBadgeAnimator.CurrentFrame
                : (_rarityIcons.TryGetValue(m.Rarity, out var icon) ? icon : null);

            var card = new SpriteCardViewModel(
                m.FileName, m.DisplayName, m.ImagePath,
                m.Group, m.VariantSuffix, m.Rarity,
                _masteredIcon!, _notMasteredIcon!)
            {
                IsCollected = data?.Collected ?? false,
                IsMastered  = data?.Mastered  ?? false,
                RarityIcon  = rarityIcon,
            };

            card.StateChanged += OnCardStateChanged;
            return card;
        }).ToList();

        await Task.WhenAll(cards.Select(async card =>
        {
            card.Image = await _imageCache.GetImageAsync(card.ImagePath);
        }));

        return cards;
    }

    private void OnCardStateChanged()
    {
        ApplyFilterAndSort();
        RecalculateStats();
        ScheduleSave();
    }

    // ── Filter and sort ──────────────────────────────────────────────────────

    private void ApplyFilterAndSort()
    {
        var filtered = _allCards.Where(MatchesSearch).Where(MatchesFilter).ToList();
        var sorted   = ApplySort(filtered).ToList();

        Func<SpriteCardViewModel, string> groupKey = GroupMode == GroupMode.ByType
            ? c => c.SpriteType : c => c.VariantCategory;

        var groupOrder = sorted
            .Select(groupKey).Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(key => GroupMode == GroupMode.ByVariant && key != "Normal")
            .ThenBy(key => key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var groups = groupOrder.Select(key =>
        {
            var g = new SpriteGroupViewModel(key, !_collapsedGroups.Contains(key));
            foreach (var card in sorted.Where(c => groupKey(c) == key))
                g.Cards.Add(card);
            g.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName != nameof(SpriteGroupViewModel.IsExpanded)) return;
                if (g.IsExpanded) _collapsedGroups.Remove(g.Header);
                else              _collapsedGroups.Add(g.Header);
            };
            return g;
        }).Where(g => g.Count > 0).ToList();

        DisplayedSprites.Clear();
        foreach (var g in groups) DisplayedSprites.Add(g);

        OverlayCards.Clear();
        foreach (var card in sorted) OverlayCards.Add(card);

        _ = OverlayExportService.WriteDataAsync(
            OverlayCards.Select(c => new OverlayCardData(c.DisplayName, c.Rarity, c.IsCollected, c.IsMastered, c.ImagePath)),
            OverlayScrollSpeed);

        IsFilteredEmpty = _allCards.Count > 0 && DisplayedSprites.Count == 0;
    }

    private bool MatchesSearch(SpriteCardViewModel c)
        => string.IsNullOrWhiteSpace(SearchText)
        || c.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

    private bool MatchesFilter(SpriteCardViewModel c) => ActiveFilter switch
    {
        FilterMode.Collected    =>  c.IsCollected,
        FilterMode.NotCollected => !c.IsCollected,
        FilterMode.Mastered     =>  c.IsMastered,
        FilterMode.NotMastered  => !c.IsMastered,
        FilterMode.Incomplete   => !(c.IsCollected && c.IsMastered),
        FilterMode.Completed    =>   c.IsCollected && c.IsMastered,
        _                       => true,
    };

    private static readonly Dictionary<string, int> RarityOrder = new(StringComparer.OrdinalIgnoreCase)
        { ["RARE"]=0, ["EPIC"]=1, ["LEGENDARY"]=2, ["MYTHIC"]=3, ["SPECIAL"]=4 };

    private IEnumerable<SpriteCardViewModel> ApplySort(List<SpriteCardViewModel> cards)
        => (SortMode)SelectedSortIndex switch
        {
            SortMode.NameAZ            => cards.OrderBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.NameZA            => cards.OrderByDescending(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.CollectedFirst    => cards.OrderByDescending(c => c.IsCollected).ThenBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.NotCollectedFirst => cards.OrderBy(c => c.IsCollected).ThenBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.MasteredFirst     => cards.OrderByDescending(c => c.IsMastered).ThenBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.UnmasteredFirst   => cards.OrderBy(c => c.IsMastered).ThenBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.RarityFirst       => cards.OrderBy(c => RarityOrder.TryGetValue(c.Rarity, out var r) ? r : 99)
                                               .ThenBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            _                          => cards,
        };

    // ── Stats ────────────────────────────────────────────────────────────────

    private void RecalculateStats()
    {
        var total     = _allCards.Count;
        var collected = _allCards.Count(c => c.IsCollected);
        var mastered  = _allCards.Count(c => c.IsMastered);

        TotalCount               = total;
        CollectedCount           = collected;
        MasteredCount            = mastered;
        CollectionProgress       = total > 0 ? (double)collected / total : 0;
        MasteryProgress          = total > 0 ? (double)mastered  / total : 0;
        CollectionPercentageText = $"{CollectionProgress:P0}";
        MasteryPercentageText    = $"{MasteryProgress:P0}";
        IsEmpty                  = total == 0;
    }

    // ── Save ─────────────────────────────────────────────────────────────────

    private void ScheduleSave()
    {
        _saveCts?.Cancel();
        _saveCts?.Dispose();
        _saveCts = new CancellationTokenSource();

        var token    = _saveCts.Token;
        var snapshot = BuildSaveSnapshot();

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(150, token);
                await _collectionData.SaveAsync(snapshot);
                await Dispatcher.UIThread.InvokeAsync(() => StatusMessage = "Saved ✓");
                await Task.Delay(2000, token);
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (StatusMessage == "Saved ✓") StatusMessage = string.Empty;
                });
            }
            catch (OperationCanceledException) { }
            catch { }
        }, token);
    }

    private Dictionary<string, SpriteData> BuildSaveSnapshot()
        => _allCards.ToDictionary(
               c => c.FileName,
               c => new SpriteData { Collected = c.IsCollected, Mastered = c.IsMastered });
}
