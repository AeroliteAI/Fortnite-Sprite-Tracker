using System.Net;
using System.Text;

namespace FortniteSpriteTracker.Services;

// Minimal HTTP server that serves overlay card data and sprite images to OBS browser source.
// Runs on http://localhost:7842/ — no admin rights needed for localhost.
public static class OverlayHttpServer
{
    private static HttpListener? _listener;
    private static volatile string _cardJson = "[]";
    private const int Port = 7842;

    public static void Start()
    {
        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{Port}/");
            _listener.Start();
            Task.Run(ServeLoop);
        }
        catch { }
    }

    public static void UpdateData(string json) => _cardJson = json;

    public static void Stop()
    {
        try { _listener?.Stop(); _listener?.Close(); }
        catch { }
        _listener = null;
    }

    private static async Task ServeLoop()
    {
        while (_listener?.IsListening == true)
        {
            try
            {
                var ctx = await _listener.GetContextAsync();
                var req  = ctx.Request;
                var res  = ctx.Response;
                res.Headers.Add("Access-Control-Allow-Origin", "*");

                var path = req.Url?.AbsolutePath ?? "/";

                if (path == "/data")
                {
                    // JSON card list
                    var bytes = Encoding.UTF8.GetBytes(_cardJson);
                    res.ContentType     = "application/json";
                    res.ContentLength64 = bytes.Length;
                    await res.OutputStream.WriteAsync(bytes);
                }
                else if (path.StartsWith("/sprite/"))
                {
                    var filePath = Uri.UnescapeDataString(path["/sprite/".Length..]);
                    var fullPath = Path.GetFullPath(filePath);
                    // Only serve files that live inside the app's own Sprites or Assets folders
                    var spritesRoot = Path.GetFullPath(AppPaths.Sprites);
                    var assetsRoot  = Path.GetFullPath(AppPaths.Assets);
                    var allowed = fullPath.StartsWith(spritesRoot, StringComparison.OrdinalIgnoreCase)
                               || fullPath.StartsWith(assetsRoot,  StringComparison.OrdinalIgnoreCase);
                    if (allowed && File.Exists(fullPath))
                    {
                        var bytes = await File.ReadAllBytesAsync(fullPath);
                        res.ContentType     = "image/webp";
                        res.ContentLength64 = bytes.Length;
                        await res.OutputStream.WriteAsync(bytes);
                    }
                    else res.StatusCode = 403;
                }
                else if (path.StartsWith("/asset/"))
                {
                    // Assets (badges, crown icons) — validated to stay within Assets folder
                    var name     = Uri.UnescapeDataString(path["/asset/".Length..]);
                    var fullPath = Path.GetFullPath(Path.Combine(AppPaths.Assets, name));
                    var root     = Path.GetFullPath(AppPaths.Assets);
                    if (fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) && File.Exists(fullPath))
                    {
                        var bytes = await File.ReadAllBytesAsync(fullPath);
                        var ext   = Path.GetExtension(fullPath).ToLowerInvariant();
                        res.ContentType     = ext == ".webp" ? "image/webp" : "image/png";
                        res.ContentLength64 = bytes.Length;
                        await res.OutputStream.WriteAsync(bytes);
                    }
                    else res.StatusCode = 404;
                }
                else res.StatusCode = 404;

                res.Close();
            }
            catch { }
        }
    }
}
