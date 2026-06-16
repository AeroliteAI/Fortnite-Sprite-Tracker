using SkiaSharp;

namespace FortniteSpriteTracker.Services;

public class ImageCacheService
{
    private readonly Dictionary<string, WeakReference<BitmapSource>> _cache
        = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Lazy<BitmapSource> _placeholder = new(CreatePlaceholder);

    // SkiaSharp's native WebP decoder isn't safe to call from many threads at once —
    // concurrent SKBitmap.Decode calls have caused native heap corruption crashes.
    // Gate all decodes to one at a time; decoding ~40 small sprites sequentially is still fast.
    private static readonly SemaphoreSlim _decodeGate = new(1, 1);

    /// <summary>
    /// Decodes a WebP sprite image. CPU work runs on a background thread;
    /// BitmapSource creation happens on the calling (UI) thread after the await.
    /// </summary>
    public async Task<BitmapSource> GetImageAsync(string path)
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

        BitmapSource bitmap;
        if (pixels is not null)
        {
            var (buffer, width, height, stride) = pixels.Value;
            bitmap = BitmapSource.Create(width, height, 96, 96,
                PixelFormats.Bgra32, null, buffer, stride);
            bitmap.Freeze();
        }
        else
        {
            bitmap = _placeholder.Value;
        }

        _cache[path] = new WeakReference<BitmapSource>(bitmap);
        return bitmap;
    }

    /// <summary>
    /// Loads a standard PNG/JPG crown image using WPF's native decoder.
    /// Must be called on the UI thread.
    /// </summary>
    public static BitmapSource LoadCrownImage(string path)
    {
        if (!File.Exists(path))
            return _placeholder.Value;

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource   = new Uri(path, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        catch
        {
            return _placeholder.Value;
        }
    }

    // Runs on background thread — returns raw BGRA pixel data; no WPF objects created here.
    private static (byte[] buffer, int width, int height, int stride)? TryDecodePixels(string path)
    {
        if (!File.Exists(path))
            return null;

        try
        {
            using var decoded = SKBitmap.Decode(path);
            if (decoded is null)
                return null;

            // Ensure BGRA8888 layout so bytes map directly to WPF's Bgra32 pixel format
            SKBitmap? converted = null;
            var source = decoded;
            if (decoded.ColorType != SKColorType.Bgra8888)
            {
                converted = decoded.Copy(SKColorType.Bgra8888);
                if (converted is null)
                    return null;
                source = converted;
            }

            try
            {
                return (source.Bytes, source.Width, source.Height, source.RowBytes);
            }
            finally
            {
                converted?.Dispose();
            }
        }
        catch
        {
            return null;
        }
    }

    private static BitmapSource CreatePlaceholder()
    {
        var bmp = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgra32, null,
            new byte[] { 42, 42, 60, 255 }, 4);
        bmp.Freeze();
        return bmp;
    }
}
