using System.Text.Json;
using FortniteSpriteTracker.Models;

namespace FortniteSpriteTracker.Services;

public class AppSettingsService
{
    private static readonly string Path =
        System.IO.Path.Combine(AppPaths.Data, "appSettings.json");

    private static readonly JsonSerializerOptions JsonOptions =
        new() { WriteIndented = true };

    public async Task<AppSettings> LoadAsync()
    {
        if (!File.Exists(Path)) return new AppSettings();
        try
        {
            var json = await File.ReadAllTextAsync(Path);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch { return new AppSettings(); }
    }

    public async Task SaveAsync(AppSettings settings)
    {
        Directory.CreateDirectory(AppPaths.Data);
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        await File.WriteAllTextAsync(Path, json);
    }
}
