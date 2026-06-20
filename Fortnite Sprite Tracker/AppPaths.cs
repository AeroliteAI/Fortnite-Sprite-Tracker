namespace FortniteSpriteTracker;

internal static class AppPaths
{
    public static readonly string Base   = AppContext.BaseDirectory;
    public static readonly string Assets = Path.Combine(Base, "Assets");
    public static readonly string Sprites = Path.Combine(Base, "Sprites");
    public static readonly string Data    = Path.Combine(Base, "Data");

    public static readonly string Mastered    = Path.Combine(Assets, "Mastered.png");
    public static readonly string NotMastered = Path.Combine(Assets, "NotMastered.png");
    public static readonly string AppIcon     = Path.Combine(Assets, "FNTracker.ico");

    public static readonly string OverlayData = Path.Combine(Base, "overlay_data.json");
    public static readonly string OverlayHtml = Path.Combine(Base, "overlay.html");
}
