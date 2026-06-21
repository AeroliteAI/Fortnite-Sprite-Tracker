using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace FortniteSpriteTracker.Services;

public class ImageCacheService
{
    private readonly Dictionary<string, WeakReference<Bitmap>> _cache
        = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Lazy<Bitmap> _placeholder = new(CreatePlaceholder);

    // SkiaSharp's native WebP decoder isn't safe to call from many threads at once.
    private static readonly SemaphoreSlim _decodeGate = new(1, 1);

    public async Task<Bitmap> GetImageAsync(string path)
    {
        if (_cache.TryGetValue(path, out var weakRef) && weakRef.TryGetTarget(out var cached))
            return cached;

        await _decodeGate.WaitAsync();
        (byte[] buffer, int width, int height, int stride)? pixels;
        try
        {
            pixels = await Task.Run(() => TryDecodePixels(path));
        }
        finally
        {
            _decodeGate.Release();
        }

        var bitmap = pixels is not null
            ? CreateBitmap(pixels.Value.buffer, pixels.Value.width, pixels.Value.height, pixels.Value.stride)
            : _placeholder.Value;

        _cache[path] = new WeakReference<Bitmap>(bitmap);
        return bitmap;
    }

    // Loads a PNG from the embedded AvaloniaResource (avares://FortniteSpriteTracker/Assets/...)
    public static Bitmap LoadAssetImage(string assetFileName)
    {
        try
        {
            var uri    = new Uri($"avares://FortniteSpriteTracker/Assets/{assetFileName}");
            using var stream = Avalonia.Platform.AssetLoader.Open(uri);
            return new Bitmap(stream);
        }
        catch { return _placeholder.Value; }
    }

    private static Bitmap CreateBitmap(byte[] buffer, int width, int height, int stride)
    {
        var bmp = new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Unpremul);
        using var fb = bmp.Lock();
        Marshal.Copy(buffer, 0, fb.Address, buffer.Length);
        return bmp;
    }

    private static (byte[] buffer, int width, int height, int stride)? TryDecodePixels(string path)
    {
        if (!File.Exists(path)) return null;
        try
        {
            using var decoded = SKBitmap.Decode(path);
            if (decoded is null) return null;

            SKBitmap? converted = null;
            var source = decoded;
            if (decoded.ColorType != SKColorType.Bgra8888)
            {
                converted = decoded.Copy(SKColorType.Bgra8888);
                if (converted is null) return null;
                source = converted;
            }

            try   { return (source.Bytes, source.Width, source.Height, source.RowBytes); }
            finally { converted?.Dispose(); }
        }
        catch { return null; }
    }

    private static Bitmap CreatePlaceholder()
        => CreateBitmap(new byte[] { 42, 42, 60, 255 }, 1, 1, 4);
}
