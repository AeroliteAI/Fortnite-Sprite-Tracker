namespace FortniteSpriteTracker;

internal static class AppPaths
{
    // Install directory — read-only in Program Files; contains Assets and the exe
    public static readonly string Base   = AppContext.BaseDirectory;
    public static readonly string Assets = Path.Combine(Base, "Assets");

    // User data directory — writable; contains Sprites the user drops in + save data
    public static readonly string UserData = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "FortniteSpriteTracker");
    public static readonly string Sprites = Path.Combine(UserData, "Sprites");
    public static readonly string Data    = Path.Combine(UserData, "Data");

    // Asset files shipped with the app
    public static readonly string Mastered    = Path.Combine(Assets, "Mastered.png");
    public static readonly string NotMastered = Path.Combine(Assets, "NotMastered.png");
    public static readonly string AppIcon     = Path.Combine(Assets, "FNTracker.ico");
}
