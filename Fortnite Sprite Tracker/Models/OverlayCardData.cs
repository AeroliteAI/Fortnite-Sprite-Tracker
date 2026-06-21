namespace FortniteSpriteTracker.Models;

public record OverlayCardData(
    string DisplayName,
    string Rarity,
    bool   IsCollected,
    bool   IsMastered,
    string ImagePath);
