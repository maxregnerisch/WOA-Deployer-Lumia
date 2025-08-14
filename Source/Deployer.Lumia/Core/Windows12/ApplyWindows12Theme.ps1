# Windows 12 Concept UI Theme Applier
# This script applies Windows 12 concept UI modifications to a Windows installation

param (
    [Parameter(Mandatory=$true)]
    [string]$WindowsPath
)

Write-Output "Applying Windows 12 Concept UI to Windows installation at $WindowsPath"

# Create Windows 12 theme directory
$themeDir = Join-Path $WindowsPath "Windows\Resources\Themes\Windows12"
New-Item -Path $themeDir -ItemType Directory -Force | Out-Null

# Registry modifications for Windows 12 UI
$registryPath = Join-Path $WindowsPath "Windows\System32\config\SOFTWARE"
$tempHive = "HKLM\Windows12Temp"

# Load the SOFTWARE hive
reg load $tempHive $registryPath

# Apply Windows 12 UI settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Themes" /v "CurrentTheme" /d "%SystemRoot%\Resources\Themes\Windows12\Windows12.theme" /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "AppsUseLightTheme" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "SystemUsesLightTheme" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "EnableTransparency" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "AccentColor" /t REG_DWORD /d 0xFF0078D7 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "ColorizationColor" /t REG_DWORD /d 0xC40078D7 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "ColorizationAfterglow" /t REG_DWORD /d 0xC40078D7 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "ColorizationGlassAttribute" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "EnableAeroPeek" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "EnableWindowColorization" /t REG_DWORD /d 1 /f

# Unload the hive
reg unload $tempHive

# Create Windows 12 theme file
$themeContent = @"
; Windows 12 Concept Theme
[Theme]
DisplayName=Windows 12 Concept
ThemeId={DE9E3F2F-A3BC-4E2D-A7A6-55B31F3E4C29}

[Control Panel\Desktop]
Wallpaper=%SystemRoot%\Web\Wallpaper\Windows\img0.jpg
TileWallpaper=0
WallpaperStyle=10
Pattern=

[VisualStyles]
Path=%SystemRoot%\Resources\Themes\Aero\Aero.msstyles
ColorStyle=NormalColor
Size=NormalSize
AutoColorization=0

[MasterThemeSelector]
MTSM=RJSPBS

[Sounds]
; IDS_SCHEME_DEFAULT
SchemeName=@mmres.dll,-800
"@

$themePath = Join-Path $themeDir "Windows12.theme"
Set-Content -Path $themePath -Value $themeContent

Write-Output "Windows 12 Concept UI theme has been applied successfully!"

