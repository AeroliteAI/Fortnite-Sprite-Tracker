using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Media.Imaging;
using FortniteSpriteTracker.Services;

namespace FortniteSpriteTracker.ViewModels;

public partial class SpriteCardViewModel : ObservableObject
{
    private readonly Bitmap _masteredIcon;
    private readonly Bitmap _notMasteredIcon;

    public string  FileName        { get; }
    public string  DisplayName     { get; }
    public string  ImagePath       { get; }
    public string  SpriteType      { get; }
    public string  VariantCategory { get; }
    public string  Rarity          { get; }
    [ObservableProperty] private Bitmap? _rarityIcon;
    public bool    HasRarity       => !string.IsNullOrEmpty(Rarity);

    [ObservableProperty] private Bitmap? _image;
    [ObservableProperty] private bool    _isCollected;
    [ObservableProperty] private bool    _isMastered;

    public Bitmap CrownImage      => IsMastered ? _masteredIcon : _notMasteredIcon;
    public IBrush CardBorderBrush => BuildBorderBrush();

    public bool JustCollected { get; private set; }
    public bool JustMastered  { get; private set; }

    public event Action? StateChanged;

    // ── Static cached brushes ────────────────────────────────────────────────

    private static readonly IBrush DefaultBorder   = new SolidColorBrush(Color.Parse("#2A2A40"));
    private static readonly IBrush CollectedBorder = new SolidColorBrush(Color.Parse("#4ADE80"));
    private static readonly IBrush MasteredBorder  = new SolidColorBrush(Color.Parse("#F59E0B"));
    private static readonly IBrush BothBorder = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint   = new RelativePoint(0, 1, RelativeUnit.Relative),
        GradientStops =
        [
            new GradientStop(Color.Parse("#4ADE80"), 0.00),
            new GradientStop(Color.Parse("#4ADE80"), 0.45),
            new GradientStop(Color.Parse("#F59E0B"), 0.55),
            new GradientStop(Color.Parse("#F59E0B"), 1.00),
        ],
    };

    private IBrush BuildBorderBrush() => (IsCollected, IsMastered) switch
    {
        (true,  true)  => BothBorder,
        (true,  false) => CollectedBorder,
        (false, true)  => MasteredBorder,
        _              => DefaultBorder,
    };

    public SpriteCardViewModel(
        string fileName,
        string displayName,
        string imagePath,
        string spriteType,
        string variantSuffix,
        string rarity,
        Bitmap masteredIcon,
        Bitmap notMasteredIcon)
    {
        FileName         = fileName;
        DisplayName      = displayName;
        ImagePath        = imagePath;
        SpriteType       = spriteType;
        VariantCategory  = string.IsNullOrEmpty(variantSuffix) ? "Normal" : variantSuffix;
        Rarity           = rarity;
        _masteredIcon    = masteredIcon;
        _notMasteredIcon = notMasteredIcon;

        if (rarity == "SPECIAL")
            SpecialBadgeAnimator.FrameChanged += OnSpecialFrameChanged;
    }

    private void OnSpecialFrameChanged() => RarityIcon = SpecialBadgeAnimator.CurrentFrame;

    internal void Cleanup()
    {
        if (Rarity == "SPECIAL")
            SpecialBadgeAnimator.FrameChanged -= OnSpecialFrameChanged;
    }

    [RelayCommand]
    private void ToggleCollected()
    {
        var v = !IsCollected;
        JustCollected = v;
        IsCollected   = v;
    }

    [RelayCommand]
    private void ToggleMastered()
    {
        var v = !IsMastered;
        JustMastered = v;
        IsMastered   = v;
    }

    public void AcknowledgeCollectedPulse() => JustCollected = false;
    public void AcknowledgeMasteredPulse()  => JustMastered  = false;

    partial void OnIsCollectedChanged(bool value)
    {
        OnPropertyChanged(nameof(CardBorderBrush));
        StateChanged?.Invoke();
    }

    partial void OnIsMasteredChanged(bool value)
    {
        OnPropertyChanged(nameof(CrownImage));
        OnPropertyChanged(nameof(CardBorderBrush));
        StateChanged?.Invoke();
    }
}
