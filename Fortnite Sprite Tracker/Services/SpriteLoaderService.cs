using FortniteSpriteTracker.Models;

namespace FortniteSpriteTracker.Services;

public class SpriteLoaderService : ISpriteLoaderService
{
    public Task<IReadOnlyList<SpriteModel>> LoadSpritesAsync(string spritesFolder)
        => Task.Run(() => LoadSprites(spritesFolder));

    private static IReadOnlyList<SpriteModel> LoadSprites(string spritesFolder)
    {
        if (!Directory.Exists(spritesFolder))
        {
            Directory.CreateDirectory(spritesFolder);
            return [];
        }

        var files = Directory.GetFiles(spritesFolder, "*.webp", SearchOption.TopDirectoryOnly);
        if (files.Length == 0)
            return [];

        var displayNames = files
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var models = new List<SpriteModel>(files.Length);

        foreach (var file in files)
        {
            var fileName    = Path.GetFileName(file);
            var displayName = Path.GetFileNameWithoutExtension(file);

            GetGroupAndSuffix(displayName, displayNames, out var group, out var suffix);

            models.Add(new SpriteModel
            {
                FileName      = fileName,
                DisplayName   = displayName,
                ImagePath     = file,
                Group         = group,
                VariantSuffix = suffix,
            });
        }

        return models
            .OrderBy(m => m.Group,         StringComparer.OrdinalIgnoreCase)
            .ThenBy(m => m.VariantSuffix != string.Empty)   // empty (Normal) sorts before any suffix
            .ThenBy(m => m.VariantSuffix,  StringComparer.OrdinalIgnoreCase)
            .ThenBy(m => m.DisplayName,    StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void GetGroupAndSuffix(
        string displayName,
        HashSet<string> allNames,
        out string group,
        out string suffix)
    {
        // If the last word stripped off reveals a name that exists as its own sprite,
        // that last word is a variant suffix. Works for any suffix automatically.
        var lastSpace = displayName.LastIndexOf(' ');
        if (lastSpace > 0)
        {
            var possibleBase   = displayName[..lastSpace];
            var possibleSuffix = displayName[(lastSpace + 1)..];

            if (allNames.Contains(possibleBase))
            {
                group  = possibleBase;
                suffix = possibleSuffix;
                return;
            }
        }

        // No matching base found — this is a base/normal sprite
        group  = displayName;
        suffix = string.Empty;
    }
}
