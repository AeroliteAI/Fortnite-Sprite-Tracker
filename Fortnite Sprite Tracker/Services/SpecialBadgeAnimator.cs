using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using SkiaSharp;

namespace FortniteSpriteTracker.Services;

public static class SpecialBadgeAnimator
{
    private const int TargetHeight = 48;
    private const int Fps          = 24;

    private static Bitmap[] _frames = [];
    private static int       _currentIndex;
    private static DispatcherTimer? _timer;

    public static event Action? FrameChanged;
    public static Bitmap? CurrentFrame =>
        _frames.Length > 0 ? _frames[_currentIndex] : null;

    public static async Task InitializeAsync(string framesFolder)
    {
        if (!Directory.Exists(framesFolder)) return;

        var files = Directory.GetFiles(framesFolder, "SPECIAL_*.webp")
                             .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                             .ToArray();
        if (files.Length == 0) return;

        var pixelData = await Task.Run(() => files.Select(TryDecodePixels).ToArray());

        var frames = new List<Bitmap>(pixelData.Length);
        foreach (var p in pixelData)
        {
            if (p is null) continue;
            frames.Add(CreateBitmap(p.Value.Buffer, p.Value.W, p.Value.H, p.Value.Stride));
        }
        if (frames.Count == 0) return;

        _frames = [.. frames];

        _timer = new DispatcherTimer(DispatcherPriority.Background)
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

    public static void SetAnimating(bool enabled)
    {
        if (_timer is null) return;
        if (enabled == _timer.IsEnabled) return;
        if (enabled) _timer.Start();
        else
        {
            _timer.Stop();
            _currentIndex = 0;
            FrameChanged?.Invoke();
        }
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
}
