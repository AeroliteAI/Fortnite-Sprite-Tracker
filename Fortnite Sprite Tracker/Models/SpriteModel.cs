namespace FortniteSpriteTracker.Models;

public class SpriteModel
{
    public required string FileName         { get; init; }
    public required string DisplayName      { get; init; }
    public required string ImagePath        { get; init; }
    public required string Group         { get; init; }
    public required string VariantSuffix { get; init; }
    public bool IsCollected { get; set; }
    public bool IsMastered  { get; set; }
}
