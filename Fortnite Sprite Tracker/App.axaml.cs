using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FortniteSpriteTracker.Services;
using FortniteSpriteTracker.ViewModels;

namespace FortniteSpriteTracker;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            EnsureFolders();

            var spriteLoader   = new SpriteLoaderService();
            var collectionData = new CollectionDataService();
            var imageCache     = new ImageCacheService();
            var appSettings    = new AppSettingsService();
            var viewModel      = new MainViewModel(spriteLoader, collectionData, imageCache, appSettings);

            var mainWindow = new MainWindow { DataContext = viewModel };
            desktop.MainWindow = mainWindow;
            mainWindow.Show();

            // InitializeAsync runs after the window is shown so HWND exists for acrylic setup
            Dispatcher.UIThread.Post(
                async () => await viewModel.InitializeAsync(),
                DispatcherPriority.Background);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void EnsureFolders()
    {
        foreach (var path in new[] { AppPaths.Sprites, AppPaths.Assets, AppPaths.Data })
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
    }
}
