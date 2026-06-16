using System.Windows.Input;
using System.Windows.Media.Animation;
using FortniteSpriteTracker.ViewModels;
using FortniteSpriteTracker.Views;

namespace FortniteSpriteTracker;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        SetWindowIcon();
        VersionText.Text = $"v{Models.PatchNotes.CurrentVersion}";
    }

    private void Options_Click(object sender, RoutedEventArgs e)
        => new OptionsWindow { Owner = this }.ShowDialog();

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && DataContext is MainViewModel vm)
            vm.ClearSearchCommand.Execute(null);
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        else
            DragMove();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState.Minimized;

    private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void Close_Click(object sender, RoutedEventArgs e)
        => Close();

    private static void SetWindowIcon()
    {
        if (!File.Exists(AppPaths.AppIcon)) return;
        try
        {
            Application.Current.MainWindow!.Icon =
                System.Windows.Media.Imaging.BitmapFrame.Create(
                    new Uri(AppPaths.AppIcon, UriKind.Absolute));
        }
        catch { /* non-fatal — window just shows default icon */ }
    }
}
