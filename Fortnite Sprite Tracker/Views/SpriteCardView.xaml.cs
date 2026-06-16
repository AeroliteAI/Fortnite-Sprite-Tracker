using System.Windows.Media.Animation;
using FortniteSpriteTracker.ViewModels;

namespace FortniteSpriteTracker.Views;

public partial class SpriteCardView : System.Windows.Controls.UserControl
{
    public SpriteCardView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not SpriteCardViewModel vm) return;

        if (vm.JustCollected) { vm.AcknowledgeCollectedPulse(); Pulse(CollectedPulseGlow); }
        if (vm.JustMastered)  { vm.AcknowledgeMasteredPulse();  Pulse(MasteredPulseGlow); }
    }

    private static void Pulse(System.Windows.Controls.Border glow)
    {
        var animation = new DoubleAnimation
        {
            From = 0,
            To = 0.85,
            Duration = TimeSpan.FromSeconds(0.25),
            AutoReverse = true,
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        };
        glow.BeginAnimation(System.Windows.Controls.Border.OpacityProperty, animation);
    }
}
