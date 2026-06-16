using FortniteSpriteTracker.Models;

namespace FortniteSpriteTracker.Services;

public class SpriteLoaderService : ISpriteLoaderService
{
    private static readonly HashSet<string> RarityKeywords =
        new(StringComparer.OrdinalIgnoreCase) { "RARE", "EPIC", "LEGENDARY", "MYTHIC", "SPECIAL" };

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

        // First pass: strip rarity from every filename so the name set used for
        // variant detection never contains rarity keywords.
        var fileInfos = files
            .Select(f =>
            {
                var raw            = Path.GetFileNameWithoutExtension(f);
                var (display, rarity) = StripRarity(raw);
                return (file: f, display, rarity);
            })
            .ToList();

        var allDisplayNames = fileInfos
            .Select(fi => fi.display)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var models = new List<SpriteModel>(files.Length);

        foreach (var (file, display, rarity) in fileInfos)
        {
            GetGroupAndSuffix(display, allDisplayNames, out var group, out var suffix);

            models.Add(new SpriteModel
            {
                FileName      = Path.GetFileName(file),
                DisplayName   = display,
                ImagePath     = file,
                Group         = group,
                VariantSuffix = suffix,
                Rarity        = rarity,
            });
        }

        return models
            .OrderBy(m => m.Group,         StringComparer.OrdinalIgnoreCase)
            .ThenBy(m => m.VariantSuffix != string.Empty)
            .ThenBy(m => m.VariantSuffix,  StringComparer.OrdinalIgnoreCase)
            .ThenBy(m => m.DisplayName,    StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    // Removes the rarity keyword from the raw filename.
    // "Fire Sprite RARE"          → ("Fire Sprite",       "RARE")
    // "Demon Sprite SPECIAL Gold" → ("Demon Sprite Gold", "SPECIAL")
    // "Demon Sprite Gold"         → ("Demon Sprite Gold", "")
    private static (string display, string rarity) StripRarity(string rawName)
    {
        var words = rawName.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (!RarityKeywords.Contains(words[i])) continue;
            var rarity  = words[i].ToUpperInvariant();
            var display = string.Join(' ', words.Where((_, idx) => idx != i)).Trim();
            return (display, rarity);
        }
        return (rawName, string.Empty);
    }

    private static void GetGroupAndSuffix(
        string displayName,
        HashSet<string> allNames,
        out string group,
        out string suffix)
    {
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

        group  = displayName;
        suffix = string.Empty;
    }
}
