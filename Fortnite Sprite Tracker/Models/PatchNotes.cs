namespace FortniteSpriteTracker.Models;

public class PatchNoteItem
{
    public required string Tag  { get; init; } // "Added", "Changed", "Fixed", "Removed"
    public required string Text { get; init; }
}

public class PatchNoteEntry
{
    public required string Version { get; init; }
    public required string Date    { get; init; }
    public required List<PatchNoteItem> Items { get; init; }
}

public static class PatchNotes
{
    public const string CurrentVersion = "0.2.2";

    public static readonly List<PatchNoteEntry> History =
    [
        new PatchNoteEntry
        {
            Version = "0.2.2",
            Date    = "2026-06-21",
            Items =
            [
                new PatchNoteItem { Tag = "Changed", Text = "Server status indicator dot reduced to 50% size." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.2.1",
            Date    = "2026-06-20",
            Items =
            [
                new PatchNoteItem { Tag = "Fixed",   Text = "App icon now shows correctly in taskbar and window." },
                new PatchNoteItem { Tag = "Fixed",   Text = "Clicking outside the search box now properly dismisses focus." },
                new PatchNoteItem { Tag = "Changed", Text = "Group header count and divider line now match the variant colour." },
                new PatchNoteItem { Tag = "Added",   Text = "White sprite glow effect restored on cards." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.1.4",
            Date    = "2026-06-20",
            Items =
            [
                new PatchNoteItem { Tag = "Added", Text = "New \"Completed\" filter — shows only sprites that are both Collected and Mastered." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.1.3",
            Date    = "2026-06-20",
            Items =
            [
                new PatchNoteItem { Tag = "Added",   Text = "Acrylic glass effect on both the main window and Options window with Windows 11 rounded corners." },
                new PatchNoteItem { Tag = "Added",   Text = "Window Opacity slider in Options → Files → Display to control panel transparency." },
                new PatchNoteItem { Tag = "Added",   Text = "Layered background colors: #05080D for toolbars, #0D1621 for card grid, #213853 for Options inner cards." },
                new PatchNoteItem { Tag = "Changed", Text = "Options window now uses a custom draggable header with a close button matching the main window style." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.1.2",
            Date    = "2026-06-19",
            Items =
            [
                new PatchNoteItem { Tag = "Added",   Text = "Stream overlay server status indicator — glowing red/green dot and // Server Active / // Server Inactive label next to the version number." },
                new PatchNoteItem { Tag = "Added",   Text = "Overlay output folder setting — choose where overlay.html is saved so OBS can find it easily." },
                new PatchNoteItem { Tag = "Added",   Text = "Overlay scroll speed and server on/off toggle in Options → Files → Display." },
                new PatchNoteItem { Tag = "Changed", Text = "Overlay server is now OFF by default — enable it in Options when needed." },
                new PatchNoteItem { Tag = "Added",   Text = "Overlay now uses your custom rarity badge PNGs and crown icons instead of emoji." },
                new PatchNoteItem { Tag = "Fixed",   Text = "Overlay clears automatically in OBS when the app is closed." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.1.1",
            Date    = "2026-06-19",
            Items =
            [
                new PatchNoteItem { Tag = "Added",   Text = "Stream Overlay — click the Overlay button in the header to open a transparent always-on-top ticker for use in OBS." },
                new PatchNoteItem { Tag = "Added",   Text = "Overlay scrolls horizontally and loops infinitely. Reflects the active filter live." },
                new PatchNoteItem { Tag = "Added",   Text = "Overlay scroll speed is adjustable in Options → Files → Display." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.1.0",
            Date    = "2026-06-16",
            Items =
            [
                new PatchNoteItem { Tag = "Added", Text = "Badge animation can now be toggled on or off in Options → Files → Display." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.0.9",
            Date    = "2026-06-16",
            Items =
            [
                new PatchNoteItem { Tag = "Added", Text = "The SPECIAL rarity badge is now an animated WebP sequence with full transparency, cycling at 24fps." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.0.8",
            Date    = "2026-06-16",
            Items =
            [
                new PatchNoteItem { Tag = "Fixed", Text = "Installer now clears the Sprites folder before installing to prevent duplicate sprites on upgrade." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.0.7",
            Date    = "2026-06-16",
            Items =
            [
                new PatchNoteItem { Tag = "Added",   Text = "New \"Incomplete\" filter button — shows sprites that haven't been both collected and mastered yet." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.0.6",
            Date    = "2026-06-16",
            Items =
            [
                new PatchNoteItem { Tag = "Added", Text = "A small, low-opacity version number now shows in the bottom-right corner of the main window." },
                new PatchNoteItem { Tag = "Fixed", Text = "Resolved a rare native crash on startup caused by decoding too many sprite images at once." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.0.5",
            Date    = "2026-06-16",
            Items =
            [
                new PatchNoteItem { Tag = "Added",   Text = "Custom title bar with new Minimize / Maximize / Close buttons that grow slightly on hover." },
                new PatchNoteItem { Tag = "Changed", Text = "Removed the native Windows title bar in favor of the app's own dark header." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.0.4",
            Date    = "2026-06-15",
            Items =
            [
                new PatchNoteItem { Tag = "Added", Text = "Cards now pulse with a green or gold glow the moment they're marked Collected or Mastered." },
                new PatchNoteItem { Tag = "Fixed", Text = "Pulse glow corners now line up with the card's rounded corners." },
            ]
        },
        new PatchNoteEntry
        {
            Version = "0.0.3",
            Date    = "2026-06-15",
            Items =
            [
                new PatchNoteItem { Tag = "Added",   Text = "\"Group by Type\" toggle — group all rarity/variant versions of a sprite together." },
                new PatchNoteItem { Tag = "Added",   Text = "Group headers are now clickable to collapse or expand that group." },
                new PatchNoteItem { Tag = "Added",   Text = "Cards that are both Collected and Mastered show a half green / half gold split border." },
                new PatchNoteItem { Tag = "Changed", Text = "Mastered and Collect buttons now show a purple outline on hover instead of scaling up." },
                new PatchNoteItem { Tag = "Changed", Text = "Card hover-darken effect no longer dims the sprite art, name, or badges — only the background." },
                new PatchNoteItem { Tag = "Fixed",   Text = "Rounded corners no longer clip on the hover-darken overlay." },
                new PatchNoteItem { Tag = "Fixed",   Text = "Widened the Mastered crown button so the icon no longer clips." },
                new PatchNoteItem { Tag = "Fixed",   Text = "Updated stale example filenames in the Instructions tab." },
            ]
        },
    ];
}
