using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FortniteSpriteTracker.ViewModels;

public partial class SpriteCardViewModel : ObservableObject
{
    private readonly BitmapSource _masteredIcon;
    private readonly BitmapSource _notMasteredIcon;

    public string FileName        { get; }
    public string DisplayName     { get; }
    public string ImagePath       { get; }
    public string VariantCategory { get; }

    [ObservableProperty]
    private BitmapSource? _image;

    [ObservableProperty]
    private bool _isCollected;

    [ObservableProperty]
    private bool _isMastered;

    // Switches between the two crown images based on mastery state.
    public BitmapSource CrownImage => IsMastered ? _masteredIcon : _notMasteredIcon;

    public event Action? StateChanged;

    public SpriteCardViewModel(
        string       fileName,
        string       displayName,
        string       imagePath,
        string       variantSuffix,
        BitmapSource masteredIcon,
        BitmapSource notMasteredIcon)
    {
        FileName         = fileName;
        DisplayName      = displayName;
        ImagePath        = imagePath;
        VariantCategory  = string.IsNullOrEmpty(variantSuffix) ? "Normal" : variantSuffix;
        _masteredIcon    = masteredIcon;
        _notMasteredIcon = notMasteredIcon;
    }

    [RelayCommand]
    private void ToggleCollected() => IsCollected = !IsCollected;

    [RelayCommand]
    private void ToggleMastered() => IsMastered = !IsMastered;

    partial void OnIsCollectedChanged(bool value)
        => StateChanged?.Invoke();

    partial void OnIsMasteredChanged(bool value)
    {
        OnPropertyChanged(nameof(CrownImage));
        StateChanged?.Invoke();
    }
}
