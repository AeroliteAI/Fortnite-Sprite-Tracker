using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using FortniteSpriteTracker.Helpers;
using FortniteSpriteTracker.Models;
using FortniteSpriteTracker.ViewModels;
using FortniteSpriteTracker.Views;

namespace FortniteSpriteTracker;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;

        // Unfocus search box when clicking anywhere outside it
        AddHandler(
            Avalonia.Input.InputElement.PointerPressedEvent,
            (_, e) =>
            {
                // Walk the visual tree up from the click source
                var el = e.Source as StyledElement;
                while (el != null)
                {
                    if (el is TextBox) return; // click was inside the TextBox — keep focus
                    el = el.Parent;
                }
                TopLevel.GetTopLevel(this)?.FocusManager?.Focus(null); // click was outside — remove TextBox focus
            },
            Avalonia.Interactivity.RoutingStrategies.Tunnel,
            handledEventsToo: true);
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        if (TryGetPlatformHandle() is { } h)
            NativeMethods.EnableRoundedCorners(h.Handle);

        try
        {
            using var stream = Avalonia.Platform.AssetLoader.Open(
                new Uri("avares://FortniteSpriteTracker/Assets/FNTracker.ico"));
            Icon = new WindowIcon(stream);
        }
        catch { }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MainViewModel vm)
        {
            VersionText.Text = $"v{PatchNotes.CurrentVersion}";
            UpdateServerStatus(vm.OverlayServerEnabled);
            vm.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(vm.OverlayServerEnabled))
                    UpdateServerStatus(vm.OverlayServerEnabled);
            };
        }
    }

    private void UpdateServerStatus(bool active)
    {
        var color = active ? Color.Parse("#4ADE80") : Color.Parse("#F87171");
        var brush = new SolidColorBrush(color);
        ServerDot.Background        = brush;
        ServerDot.BoxShadow         = BoxShadows.Parse($"0 0 8 2 #{color.R:X2}{color.G:X2}{color.B:X2}");
        ServerStatusText.Foreground = brush;
        ServerStatusText.Text       = active ? "// Server Active" : "// Server Inactive";
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && DataContext is MainViewModel vm)
            vm.ClearSearchCommand.Execute(null);
    }


    private void Header_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }

    private void Resize_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        if (sender is Border b && Enum.TryParse<WindowEdge>(b.Tag?.ToString(), out var edge))
            BeginResizeDrag(edge, e);
    }

    private void Minimize_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => WindowState = WindowState.Minimized;

    private void MaximizeRestore_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => Close();

    private async void Options_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var opts = new OptionsWindow { DataContext = DataContext };
        await opts.ShowDialog(this);
    }
}
