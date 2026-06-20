using System.Text.Json;
using FortniteSpriteTracker.ViewModels;

namespace FortniteSpriteTracker.Services;

public static class OverlayExportService
{
    private static readonly JsonSerializerOptions _json = new() { WriteIndented = false };

    public static async Task WriteDataAsync(IEnumerable<SpriteCardViewModel> cards, int scrollSpeed = 80)
    {
        try
        {
            var payload = new
            {
                speed = scrollSpeed,
                cards = cards.Select(c => new
                {
                    name      = c.DisplayName,
                    rarity    = c.Rarity,
                    collected = c.IsCollected,
                    mastered  = c.IsMastered,
                    imageUrl  = "http://localhost:7842/sprite/" + Uri.EscapeDataString(c.ImagePath),
                }),
            };
            var json = JsonSerializer.Serialize(payload, _json);
            OverlayHttpServer.UpdateData(json);
            await File.WriteAllTextAsync(AppPaths.OverlayData, json);
        }
        catch { }
    }

    public static async Task WriteHtmlAsync(string? outputFolder = null)
    {
        try
        {
            var folder = string.IsNullOrWhiteSpace(outputFolder) ? AppPaths.Base : outputFolder;
            var path   = Path.Combine(folder, "overlay.html");
            Directory.CreateDirectory(folder);
            await File.WriteAllTextAsync(path, HtmlTemplate);
        }
        catch { }
    }

    private const string HtmlTemplate = """
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset="UTF-8">
        <style>
          * { margin:0; padding:0; box-sizing:border-box; }
          body { background:transparent; font-family:'Arial Black',sans-serif; overflow:hidden; }

          #viewport { overflow:hidden; width:100%; }
          #track {
            display:flex; flex-wrap:nowrap; gap:8px; padding:10px 0;
            width:max-content;
            animation:ticker linear infinite;
          }
          @keyframes ticker {
            0%   { transform:translateX(0); }
            100% { transform:translateX(-50%); }
          }

          .card {
            width:120px; background:#1A1A2E; border-radius:8px;
            border:2px solid #2A2A40; flex-shrink:0; position:relative;
            isolation:isolate;
          }
          .card.collected { border-color:#4ADE80; }
          .card.mastered  { border-color:#F59E0B; }
          .card.both      { border:2px solid transparent; background-clip:padding-box; }
          .card.both::before {
            content:''; position:absolute; inset:-2px; border-radius:10px; z-index:-1;
            background:linear-gradient(to bottom,#4ADE80 45%,#F59E0B 55%);
          }

          .img-wrap {
            width:100%; height:110px; background:#12121F; position:relative;
            overflow:hidden; border-radius:6px 6px 0 0;
          }
          .card-image { width:100%; height:100%; object-fit:contain; padding:6px; display:block; }

          .rarity-badge {
            position:absolute; top:3px; left:3px;
            height:14px; width:auto; display:block;
          }
          .check-badge {
            position:absolute; top:0; right:0;
            background:#4ADE80; color:#0D0D15; font-size:9px; font-weight:bold;
            padding:2px 4px; border-radius:0 6px 0 5px;
          }
          .card-name {
            color:#E2E8F0; font-size:10px; font-weight:bold; text-align:center;
            padding:4px 4px 2px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;
          }
          .card-bar {
            display:flex; height:36px; align-items:center; justify-content:center;
            background:#22223A; gap:6px; padding:4px;
            border-radius:0 0 6px 6px;
          }
          .crown { width:18px; height:18px; object-fit:contain; }
          .status { font-size:10px; font-weight:bold; color:#4ADE80; }
          .status.no { color:#64748B; }
        </style>
        </head>
        <body>
        <div id="viewport"><div id="track"></div></div>
        <script>
          const BASE = 'http://localhost:7842';
          const BADGE = {
            RARE:      BASE + '/asset/RARE.png',
            EPIC:      BASE + '/asset/EPIC.png',
            LEGENDARY: BASE + '/asset/LEGENDARY.png',
            MYTHIC:    BASE + '/asset/MYTHIC.png',
            SPECIAL:   BASE + '/asset/SPECIAL/SPECIAL_0000.webp',
          };
          const CROWN_ON  = BASE + '/asset/Mastered.png';
          const CROWN_OFF = BASE + '/asset/NotMastered.png';

          function makeCard(c) {
            const cls = c.collected && c.mastered ? 'both' : c.collected ? 'collected' : c.mastered ? 'mastered' : '';
            const badge = BADGE[c.rarity] ? `<img class="rarity-badge" src="${BADGE[c.rarity]}"/>` : '';
            return `<div class="card ${cls}">
              <div class="img-wrap">
                ${badge}
                ${c.collected ? '<div class="check-badge">✓</div>' : ''}
                <img class="card-image" src="${c.imageUrl}" onerror="this.style.opacity='0.15'"/>
              </div>
              <div class="card-name" title="${c.name}">${c.name}</div>
              <div class="card-bar">
                <img class="crown" src="${c.mastered ? CROWN_ON : CROWN_OFF}"/>
                <span class="status ${c.collected ? '' : 'no'}">${c.collected ? 'Collected' : 'Collect'}</span>
              </div>
            </div>`;
          }

          function render(data) {
            const cards = data.cards || [];
            const speed = data.speed || 80;
            const track = document.getElementById('track');
            if (!cards.length) { track.innerHTML = ''; return; }
            const html = cards.map(makeCard).join('');
            track.innerHTML = html + html;
            const dur = Math.max(5, (cards.length * 136) / speed);
            track.style.animationDuration = dur.toFixed(1) + 's';
          }

          function clearOverlay() {
            var t = document.getElementById('track');
            t.innerHTML = '';
            t.style.animationDuration = '0s';
          }

          function poll() {
            var x = new XMLHttpRequest();
            x.timeout = 2000;
            x.open('GET', 'http://localhost:7842/data', true);
            x.onload    = function() { try { render(JSON.parse(x.responseText)); } catch(e) { clearOverlay(); } };
            x.onerror   = function() { clearOverlay(); };
            x.ontimeout = function() { clearOverlay(); };
            x.send();
          }

          poll();
          setInterval(poll, 500);
        </script>
        </body>
        </html>
        """;
}
