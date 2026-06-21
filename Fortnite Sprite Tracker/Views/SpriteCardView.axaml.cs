using Avalonia.Controls;
using Avalonia.Input;
using FortniteSpriteTracker.ViewModels;

namespace FortniteSpriteTracker.Views;

public partial class SpriteCardView : UserControl
{
    public SpriteCardView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not SpriteCardViewModel vm) return;
        if (vm.JustCollected) { vm.AcknowledgeCollectedPulse(); _ = PulseAsync(CollectedPulseGlow); }
        if (vm.JustMastered)  { vm.AcknowledgeMasteredPulse();  _ = PulseAsync(MasteredPulseGlow); }
    }

    private static async Task PulseAsync(Border glow)
    {
        glow.Opacity = 0.85;
        await Task.Delay(300); // hold at peak (transition takes 200ms to rise)
        glow.Opacity = 0.0;
    }

    private void Card_PointerEntered(object? sender, PointerEventArgs e)
    {
        CharacterImage.Classes.Set("hovered", true);
        ImageAreaDarken.Opacity = 0.35;
    }

    private void Card_PointerExited(object? sender, PointerEventArgs e)
    {
        CharacterImage.Classes.Set("hovered", false);
        ImageAreaDarken.Opacity = 0.0;
    }
}
