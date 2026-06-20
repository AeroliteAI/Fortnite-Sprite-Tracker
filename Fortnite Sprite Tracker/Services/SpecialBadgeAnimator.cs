using SkiaSharp;
using System.Windows.Threading;

namespace FortniteSpriteTracker.Services;

// Loads the SPECIAL rarity badge WebP image sequence and drives a shared
// DispatcherTimer so every SPECIAL card animates in sync from one frame source.
public static class SpecialBadgeAnimator
{
    private const int TargetHeight = 48; // 4x the 12px display size — crisp at high DPI
    private const int Fps          = 24; // 120 frames / 5 s

    private static BitmapSource[] _frames = [];
    private static int            _currentIndex;
    private static DispatcherTimer? _timer;

    public static event Action? FrameChanged;

    public static void SetAnimating(bool enabled)
    {
        if (_timer is null) return;
        if (enabled == _timer.IsEnabled) return;
        if (enabled)
            _timer.Start();
        else
        {
            _timer.Stop();
            _currentIndex = 0;
            FrameChanged?.Invoke();
        }
    }

    public static BitmapSource? CurrentFrame =>
        _frames.Length > 0 ? _frames[_currentIndex] : null;

    public static async Task InitializeAsync(string framesFolder)
    {
        if (!Directory.Exists(framesFolder)) return;

        var files = Directory.GetFiles(framesFolder, "SPECIAL_*.webp")
                             .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                             .ToArray();
        if (files.Length == 0) return;

        // Decode + resize all frames on a background thread
        var pixelData = await Task.Run(() =>
            files.Select(TryDecodePixels).ToArray());

        // Create frozen BitmapSources on the UI thread (back here after await)
        var frames = new List<BitmapSource>(pixelData.Length);
        foreach (var p in pixelData)
        {
            if (p is null) continue;
            var bmp = BitmapSource.Create(p.Value.W, p.Value.H, 96, 96,
                PixelFormats.Bgra32, null, p.Value.Buffer, p.Value.Stride);
            bmp.Freeze();
            frames.Add(bmp);
        }
        if (frames.Count == 0) return;

        _frames = [.. frames];

        _timer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromMilliseconds(1000.0 / Fps)
        };
        _timer.Tick += (_, _) =>
        {
            _currentIndex = (_currentIndex + 1) % _frames.Length;
            FrameChanged?.Invoke();
        };
        _timer.Start();
    }

    private static (byte[] Buffer, int W, int H, int Stride)? TryDecodePixels(string path)
    {
        try
        {
            using var original = SKBitmap.Decode(path);
            if (original is null) return null;

            float scale = (float)TargetHeight / original.Height;
            int   w     = Math.Max(1, (int)(original.Width * scale));

            using var resized = original.Resize(
                new SKImageInfo(w, TargetHeight),
                new SKSamplingOptions(SKFilterMode.Linear));
            if (resized is null) return null;

            SKBitmap? converted = null;
            var source = resized;
            if (resized.ColorType != SKColorType.Bgra8888)
            {
                converted = resized.Copy(SKColorType.Bgra8888);
                if (converted is null) return null;
                source = converted;
            }

            try   { return (source.Bytes, source.Width, source.Height, source.RowBytes); }
            finally { converted?.Dispose(); }
        }
        catch { return null; }
    }
}
