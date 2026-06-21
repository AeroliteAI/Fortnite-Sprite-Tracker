# Fortnite Sprite Tracker

![Fortnite Sprite Tracker](screenshots/Main.png)

A Windows desktop app for tracking your Fortnite sprite collection. Mark sprites as Collected or Mastered, filter and sort your progress, and display your collection live on stream via an OBS browser source overlay.

Built with **Avalonia UI** for GPU-accelerated acrylic glass rendering on Windows 10/11.

---

## Features

- **Card grid** — Sprites displayed as cards grouped by variant (Normal, Galaxy, Gold, Gummy, etc.) or by type
- **Two tracking states per sprite** — Collected (green border) and Mastered (amber border). Both = split gradient border
- **Filters** — All · Collected · Not Collected · Mastered · Not Mastered · Incomplete · Completed
- **Search & Sort** — Real-time search with 8 sort modes including rarity, name, and progress-based ordering
- **Grouping** — By Variant or By Type, with collapsible group headers
- **Animated SPECIAL badge** — The SPECIAL rarity badge plays a 24fps animated WebP sequence
- **Acrylic glass UI** — GPU-accelerated frosted glass built on Avalonia UI
- **Stream overlay** — Local HTTP server on port 7842 serves an `overlay.html` browser source for OBS, showing your current filtered sprites live on stream
- **Persistent settings** — Card scale, grouping, badge animation, window opacity, and overlay settings save automatically

---

## Requirements

- Windows 10 or 11 (x64)
- [.NET 10 Runtime](https://dotnet.microsoft.com/download/dotnet/10.0)

---

## Installation

1. Download and run `FortniteSpriteTrackerSetup_vX.X.X.exe`
2. The app installs to `%LOCALAPPDATA%\Fortnite Sprite Tracker\`
3. Add your sprite `.webp` files to the **Sprites** folder (open it from Options → Files)
4. Click **Refresh** to load them

---

## File Naming

Sprites must follow this naming scheme:

| Pattern | Example | Result |
|---|---|---|
| `{Name} {RARITY}.webp` | `Burnt Peanut MYTHIC.webp` | Base sprite, MYTHIC badge |
| `{Name} {RARITY}.webp` | `Fire RARE.webp` | Base sprite, RARE badge |
| `{Name} SPECIAL {Variant}.webp` | `Fire SPECIAL Galaxy.webp` | Variant sprite, SPECIAL badge |

**Valid rarity keywords (must be ALL CAPS):** `RARE` `EPIC` `LEGENDARY` `MYTHIC` `SPECIAL`

The rarity keyword is stripped from the display name automatically. Variant suffixes (Galaxy, Gold, Gummy, etc.) create separate groups automatically — no app changes needed for new variants.

---

## Stream Overlay

The app includes a built-in browser source overlay for OBS:

1. In Options → Display, turn the **Stream Overlay Server** on
2. In OBS, add a **Browser Source** → Local File → point to `overlay.html` in the app folder
3. Check **Allow access to local files**
4. Set your preferred width and height

The overlay reflects your active filter live — switch filters in the app and OBS updates within 500ms. Scroll speed is adjustable in Options.

---

## Options

| Setting | Description |
|---|---|
| Badge Animation | Enables/disables the animated SPECIAL rarity badge |
| Window Opacity | Controls panel background transparency (acrylic glass) |
| Stream Overlay Server | Starts/stops the local HTTP server on port 7842 |
| Overlay Scroll Speed | How fast the overlay ticker scrolls |
| Overlay Output Folder | Where `overlay.html` is saved (default: app folder) |

---

## Versioning

`MAJOR.MINOR.PATCH` — patch increments each build, rolls over at 9 (0.2.9 → 0.3.0).

---

## Disclaimer

> **Use at your own risk.** This app is provided as-is with no warranty of any kind. The developer is not responsible for any damage, data loss, or issues caused to your computer or system by installing or using this software. By downloading and running this app you accept full responsibility for any consequences.

---

## Credits

Built by **Aerolite** — **Vibe Coded** using [Avalonia UI](https://avaloniaui.net/) and [SkiaSharp](https://github.com/mono/SkiaSharp).
