using FortniteSpriteTracker.Models;

namespace FortniteSpriteTracker.Services;

public interface ICollectionDataService
{
    Task<Dictionary<string, SpriteData>> LoadAsync();
    Task SaveAsync(Dictionary<string, SpriteData> data);
}
