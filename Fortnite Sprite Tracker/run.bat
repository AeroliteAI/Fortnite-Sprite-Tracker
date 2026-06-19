@echo off
taskkill /F /IM FortniteSpriteTracker.exe >nul 2>&1
timeout /t 1 /nobreak >nul
start "" "bin\Debug\net10.0-windows\FortniteSpriteTracker.exe"
