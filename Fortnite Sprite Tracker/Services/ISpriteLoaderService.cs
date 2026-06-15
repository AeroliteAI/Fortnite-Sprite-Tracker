using FortniteSpriteTracker.Models;

namespace FortniteSpriteTracker.Services;

public interface ISpriteLoaderService
{
    Task<IReadOnlyList<SpriteModel>> LoadSpritesAsync(string spritesFolder);
}
