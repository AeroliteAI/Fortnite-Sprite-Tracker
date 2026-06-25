using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FortniteSpriteTracker.ViewModels;

public partial class SpriteGroupViewModel : ObservableObject
{
    public string Header     { get; }
    public IBrush GroupBrush { get; }
    public int    Count      => Cards.Count;

    [ObservableProperty] private bool _isExpanded = true;

    public ObservableCollection<SpriteCardViewModel> Cards { get; } = [];

    public SpriteGroupViewModel(string header, bool isExpanded = true)
    {
        Header     = header;
        IsExpanded = isExpanded;
        GroupBrush = new SolidColorBrush(Avalonia.Media.Color.Parse(header switch
        {
            "Base"     => "#94A3B8",
            "Gold"     => "#F59E0B",
            "Galaxy"   => "#818CF8",
            "Gem"      => "#34D399",
            "Holofoil" => "#F0ABFC",
            "Cube"     => "#FB923C",
            "Quack"    => "#FDE68A",
            "Gummy"    => "#EC4899",
            _          => "#22D3EE",
        }));
    }

    [RelayCommand]
    private void ToggleExpanded() => IsExpanded = !IsExpanded;
}
