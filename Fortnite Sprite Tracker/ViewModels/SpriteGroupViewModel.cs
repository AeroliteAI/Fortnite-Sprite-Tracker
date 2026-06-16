using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FortniteSpriteTracker.ViewModels;

public partial class SpriteGroupViewModel : ObservableObject
{
    public string Header { get; }
    public string Color  { get; }
    public int    Count  => Cards.Count;

    [ObservableProperty] private bool _isExpanded = true;

    public ObservableCollection<SpriteCardViewModel> Cards { get; } = [];

    public SpriteGroupViewModel(string header, bool isExpanded = true)
    {
        Header = header;
        IsExpanded = isExpanded;
        Color  = header switch
        {
            "Normal" => "#94A3B8",
            "Gold"   => "#F59E0B",
            "Gummy"  => "#EC4899",
            "Galaxy" => "#818CF8",
            _        => "#22D3EE",   // cyan fallback for any future variant
        };
    }

    [RelayCommand]
    private void ToggleExpanded() => IsExpanded = !IsExpanded;
}
