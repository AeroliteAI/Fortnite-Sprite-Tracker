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
    public const string CurrentVersion = "0.0.6";

    public static readonly List<PatchNoteEntry> History =
    [
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
