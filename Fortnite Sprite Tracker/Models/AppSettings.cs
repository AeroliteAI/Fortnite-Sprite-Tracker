namespace FortniteSpriteTracker.Models;

public class AppSettings
{
    public int       CardScalePercent { get; set; } = 100;
    public GroupMode GroupMode        { get; set; } = GroupMode.ByVariant;
    public bool      AnimateBadges        { get; set; } = true;
    public int       OverlayScrollSpeed   { get; set; } = 80;
    public bool      OverlayServerEnabled { get; set; } = false;
    public string    OverlayOutputPath    { get; set; } = string.Empty;
}
