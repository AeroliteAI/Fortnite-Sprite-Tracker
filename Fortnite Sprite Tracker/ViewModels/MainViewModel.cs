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

    private List<SpriteCardViewModel> _allCards = [];
    private CancellationTokenSource?  _saveCts;

    // Crown images loaded once from Assets/ and shared across all cards
    private BitmapSource? _masteredIcon;
    private BitmapSource? _notMasteredIcon;

    public ObservableCollection<SpriteGroupViewModel> DisplayedSprites { get; } = [];


    public IReadOnlyList<string> SortOptions { get; } =
    [
        "Default (Group Order)",
        "Name A → Z",
        "Name Z → A",
        "Collected First",
        "Not Collected First",
        "Mastered First",
        "Unmastered First",
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

    public MainViewModel(
        ISpriteLoaderService   spriteLoader,
        ICollectionDataService collectionData,
        ImageCacheService      imageCache)
    {
        _spriteLoader   = spriteLoader;
        _collectionData = collectionData;
        _imageCache     = imageCache;
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;

        // Crown images must be created on the UI thread (BitmapImage uses WPF internals)
        _masteredIcon    = ImageCacheService.LoadCrownImage(AppPaths.Mastered);
        _notMasteredIcon = ImageCacheService.LoadCrownImage(AppPaths.NotMastered);

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
        IsLoading = true;

        // Snapshot current in-memory state so existing progress is preserved
        var snapshot     = BuildSaveSnapshot();
        var spriteModels = await _spriteLoader.LoadSpritesAsync(AppPaths.Sprites);

        foreach (var card in _allCards)
            card.StateChanged -= OnCardStateChanged;

        _allCards = await BuildCardsAsync(spriteModels, snapshot);

        ApplyFilterAndSort();
        RecalculateStats();

        IsLoading = false;
    }

    [RelayCommand] private void FilterAll()          => ActiveFilter = FilterMode.All;
    [RelayCommand] private void FilterCollected()    => ActiveFilter = FilterMode.Collected;
    [RelayCommand] private void FilterNotCollected() => ActiveFilter = FilterMode.NotCollected;
    [RelayCommand] private void FilterMastered()     => ActiveFilter = FilterMode.Mastered;
    [RelayCommand] private void FilterNotMastered()  => ActiveFilter = FilterMode.NotMastered;
    [RelayCommand] private void ClearSearch()        => SearchText   = string.Empty;

    // ── Property change hooks ────────────────────────────────────────────────

    partial void OnSearchTextChanged(string value)       => ApplyFilterAndSort();
    partial void OnActiveFilterChanged(FilterMode value) => ApplyFilterAndSort();
    partial void OnSelectedSortIndexChanged(int value)   => ApplyFilterAndSort();

    // ── Card building ────────────────────────────────────────────────────────

    private async Task<List<SpriteCardViewModel>> BuildCardsAsync(
        IReadOnlyList<SpriteModel>      models,
        Dictionary<string, SpriteData>  saveData)
    {
        var cards = models.Select(m =>
        {
            saveData.TryGetValue(m.FileName, out var data);

            var card = new SpriteCardViewModel(
                m.FileName,
                m.DisplayName,
                m.ImagePath,
                m.VariantSuffix,
                _masteredIcon!,
                _notMasteredIcon!)
            {
                IsCollected = data?.Collected ?? false,
                IsMastered  = data?.Mastered  ?? false,
            };

            card.StateChanged += OnCardStateChanged;
            return card;
        }).ToList();

        // Decode all WebP images in parallel on background threads;
        // BitmapSource.Create is called on the UI thread after each await.
        await Task.WhenAll(cards.Select(async card =>
        {
            card.Image = await _imageCache.GetImageAsync(card.ImagePath);
        }));

        return cards;
    }

    // ── Card state change ────────────────────────────────────────────────────

    private void OnCardStateChanged()
    {
        // Re-filter so cards with changed state can appear/disappear instantly
        // (e.g. if filter is "Collected" and user uncollects a card)
        ApplyFilterAndSort();
        RecalculateStats();
        ScheduleSave();
    }

    // ── Filter and sort ──────────────────────────────────────────────────────

    private void ApplyFilterAndSort()
    {
        var filtered = _allCards
            .Where(MatchesSearch)
            .Where(MatchesFilter)
            .ToList();

        var sorted = ApplySort(filtered).ToList();

        // Derive category order from the actual data: Normal first, then all other suffixes A-Z
        var categoryOrder = sorted
            .Select(c => c.VariantCategory)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(cat => cat != "Normal")              // Normal sorts before everything else
            .ThenBy(cat => cat, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var groups = categoryOrder
            .Select(category =>
            {
                var group = new SpriteGroupViewModel(category);
                foreach (var card in sorted.Where(c => c.VariantCategory == category))
                    group.Cards.Add(card);
                return group;
            })
            .Where(g => g.Count > 0)
            .ToList();

        DisplayedSprites.Clear();
        foreach (var group in groups)
            DisplayedSprites.Add(group);

        IsFilteredEmpty = _allCards.Count > 0 && DisplayedSprites.Count == 0;
    }

    private bool MatchesSearch(SpriteCardViewModel card)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return true;
        return card.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
    }

    private bool MatchesFilter(SpriteCardViewModel card) => ActiveFilter switch
    {
        FilterMode.Collected    =>  card.IsCollected,
        FilterMode.NotCollected => !card.IsCollected,
        FilterMode.Mastered     =>  card.IsMastered,
        FilterMode.NotMastered  => !card.IsMastered,
        _                       => true,
    };

    private IEnumerable<SpriteCardViewModel> ApplySort(List<SpriteCardViewModel> cards)
        => (SortMode)SelectedSortIndex switch
        {
            SortMode.NameAZ            => cards.OrderBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.NameZA            => cards.OrderByDescending(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.CollectedFirst    => cards.OrderByDescending(c => c.IsCollected).ThenBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.NotCollectedFirst => cards.OrderBy(c => c.IsCollected).ThenBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.MasteredFirst     => cards.OrderByDescending(c => c.IsMastered).ThenBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            SortMode.UnmasteredFirst   => cards.OrderBy(c => c.IsMastered).ThenBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase),
            _                          => cards, // Default — already group-sorted by SpriteLoaderService
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
        var snapshot = BuildSaveSnapshot(); // capture state now, not after delay

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(150, token);
                await _collectionData.SaveAsync(snapshot);
                await Application.Current.Dispatcher.InvokeAsync(() => StatusMessage = "Saved ✓");
                await Task.Delay(2000);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (StatusMessage == "Saved ✓") StatusMessage = string.Empty;
                });
            }
            catch (OperationCanceledException) { }
            catch (Exception) { /* non-fatal; backup protects against data loss */ }
        }, token);
    }

    private Dictionary<string, SpriteData> BuildSaveSnapshot()
        => _allCards.ToDictionary(
               c => c.FileName,
               c => new SpriteData { Collected = c.IsCollected, Mastered = c.IsMastered });
}
