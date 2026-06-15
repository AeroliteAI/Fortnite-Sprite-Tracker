namespace FortniteSpriteTracker.ViewModels;

public class SpriteGroupViewModel
{
    public string Header { get; }
    public string Color  { get; }
    public int    Count  => Cards.Count;

    public ObservableCollection<SpriteCardViewModel> Cards { get; } = [];

    public SpriteGroupViewModel(string header)
    {
        Header = header;
        Color  = header switch
        {
            "Normal" => "#94A3B8",
            "Gold"   => "#F59E0B",
            "Gummy"  => "#EC4899",
            "Galaxy" => "#818CF8",
            _        => "#22D3EE",   // cyan fallback for any future variant
        };
    }
}
