using System.Text.Json;
using FortniteSpriteTracker.Models;

namespace FortniteSpriteTracker.Services;

public class CollectionDataService : ICollectionDataService
{
    private static readonly string SavePath   = Path.Combine(AppPaths.Data, "collectionData.json");
    private static readonly string BackupPath = Path.Combine(AppPaths.Data, "collectionData.backup.json");
    private static readonly string TempPath   = Path.Combine(AppPaths.Data, "collectionData.tmp.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented               = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<Dictionary<string, SpriteData>> LoadAsync()
    {
        EnsureDataFolder();

        // Try primary save file first
        var result = await TryLoadAsync(SavePath);
        if (result is not null)
            return result;

        // Primary failed or corrupt — restore from backup
        result = await TryLoadAsync(BackupPath);
        if (result is not null)
        {
            try { File.Copy(BackupPath, SavePath, overwrite: true); } catch { /* non-fatal */ }
            return result;
        }

        // Both missing or corrupt — start fresh
        return new Dictionary<string, SpriteData>();
    }

    public async Task SaveAsync(Dictionary<string, SpriteData> data)
    {
        await _lock.WaitAsync();
        try
        {
            EnsureDataFolder();

            var json = JsonSerializer.Serialize(data, JsonOptions);

            // Write to temp file first
            await File.WriteAllTextAsync(TempPath, json);

            // Promote current save to backup before replacing
            if (File.Exists(SavePath))
                File.Copy(SavePath, BackupPath, overwrite: true);

            // Atomic replace
            File.Move(TempPath, SavePath, overwrite: true);
        }
        finally
        {
            _lock.Release();
        }
    }

    private static async Task<Dictionary<string, SpriteData>?> TryLoadAsync(string path)
    {
        if (!File.Exists(path))
            return null;

        try
        {
            var json = await File.ReadAllTextAsync(path);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.Deserialize<Dictionary<string, SpriteData>>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private static void EnsureDataFolder()
    {
        if (!Directory.Exists(AppPaths.Data))
            Directory.CreateDirectory(AppPaths.Data);
    }
}
