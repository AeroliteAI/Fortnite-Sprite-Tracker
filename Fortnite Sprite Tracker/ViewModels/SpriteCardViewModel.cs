using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FortniteSpriteTracker.Services;

namespace FortniteSpriteTracker.ViewModels;

public partial class SpriteCardViewModel : ObservableObject
{
    private readonly BitmapSource _masteredIcon;
    private readonly BitmapSource _notMasteredIcon;

    public string        FileName        { get; }
    public string        DisplayName     { get; }
    public string        ImagePath       { get; }
    public string        SpriteType      { get; }
    public string        VariantCategory { get; }
    public string        Rarity          { get; }
    [ObservableProperty] private BitmapSource? _rarityIcon;
    public bool          HasRarity       => !string.IsNullOrEmpty(Rarity);

    [ObservableProperty] private BitmapSource? _image;
    [ObservableProperty] private bool _isCollected;
    [ObservableProperty] private bool _isMastered;

    public BitmapSource CrownImage => IsMastered ? _masteredIcon : _notMasteredIcon;

    public bool JustCollected { get; private set; }
    public bool JustMastered  { get; private set; }

    public event Action? StateChanged;

    public SpriteCardViewModel(
        string       fileName,
        string       displayName,
        string       imagePath,
        string       spriteType,
        string       variantSuffix,
        string       rarity,
        BitmapSource masteredIcon,
        BitmapSource notMasteredIcon)
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

    private void OnSpecialFrameChanged()
        => RarityIcon = SpecialBadgeAnimator.CurrentFrame;

    internal void Cleanup()
    {
        if (Rarity == "SPECIAL")
            SpecialBadgeAnimator.FrameChanged -= OnSpecialFrameChanged;
    }

    [RelayCommand]
    private void ToggleCollected()
    {
        var newValue = !IsCollected;
        JustCollected = newValue;
        IsCollected = newValue;
    }

    [RelayCommand]
    private void ToggleMastered()
    {
        var newValue = !IsMastered;
        JustMastered = newValue;
        IsMastered = newValue;
    }

    public void AcknowledgeCollectedPulse() => JustCollected = false;
    public void AcknowledgeMasteredPulse()  => JustMastered  = false;

    partial void OnIsCollectedChanged(bool value) => StateChanged?.Invoke();

    partial void OnIsMasteredChanged(bool value)
    {
        OnPropertyChanged(nameof(CrownImage));
        StateChanged?.Invoke();
    }
}
