using FortniteSpriteTracker.Services;
using FortniteSpriteTracker.ViewModels;

namespace FortniteSpriteTracker;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        EnsureFolders();

        var spriteLoader   = new SpriteLoaderService();
        var collectionData = new CollectionDataService();
        var imageCache     = new ImageCacheService();
        var viewModel      = new MainViewModel(spriteLoader, collectionData, imageCache);

        var mainWindow = new MainWindow(viewModel);
        mainWindow.Show();

        await viewModel.InitializeAsync();
    }

    private static void EnsureFolders()
    {
        foreach (var path in new[] { AppPaths.Sprites, AppPaths.Assets, AppPaths.Data })
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
