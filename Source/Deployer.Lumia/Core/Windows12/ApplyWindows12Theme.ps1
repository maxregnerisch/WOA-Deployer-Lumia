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

# Create Windows 12 cursors directory
$cursorsDir = Join-Path $themeDir "Cursors"
New-Item -Path $cursorsDir -ItemType Directory -Force | Out-Null

# Create Windows 12 wallpapers directory
$wallpapersDir = Join-Path $WindowsPath "Windows\Web\Wallpaper\Windows12"
New-Item -Path $wallpapersDir -ItemType Directory -Force | Out-Null

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
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "ColorPrevalence" /t REG_DWORD /d 1 /f

# DWM settings
reg add "$tempHive\Microsoft\Windows\DWM" /v "AccentColor" /t REG_DWORD /d 0xFF0078D7 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "ColorizationColor" /t REG_DWORD /d 0xC40078D7 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "ColorizationAfterglow" /t REG_DWORD /d 0xC40078D7 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "ColorizationGlassAttribute" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "EnableAeroPeek" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "EnableWindowColorization" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "AccentColorInactive" /t REG_DWORD /d 0xFF505050 /f
reg add "$tempHive\Microsoft\Windows\DWM" /v "ColorPrevalence" /t REG_DWORD /d 1 /f

# Taskbar settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarAl" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarMn" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ShowTaskViewButton" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarDa" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarSi" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarSmallIcons" /t REG_DWORD /d 0 /f

# Start menu settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Start_Layout" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "StartMFUEnabled" /t REG_DWORD /d 0 /f

# Explorer settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "LaunchTo" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "HideFileExt" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Hidden" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ShowSyncProviderNotifications" /t REG_DWORD /d 0 /f

# Desktop settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "IconsOnly" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ListviewAlphaSelect" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ListviewShadow" /t REG_DWORD /d 1 /f

# System settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "EnableSnapAssistFlyout" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "EnableTaskGroups" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ExtendedUIHoverTime" /t REG_DWORD /d 1 /f

# Windows 12 specific settings
reg add "$tempHive\Microsoft\Windows NT\CurrentVersion" /v "ProductName" /d "Windows 12 Pro" /f
reg add "$tempHive\Microsoft\Windows NT\CurrentVersion" /v "EditionID" /d "Professional" /f
reg add "$tempHive\Microsoft\Windows NT\CurrentVersion\Windows" /v "DisplayVersion" /d "24H2" /f

# Unload the hive
reg unload $tempHive

# Create Windows 12 theme file
$themeContent = @"
; Windows 12 Concept Theme
[Theme]
DisplayName=Windows 12 Concept
ThemeId={DE9E3F2F-A3BC-4E2D-A7A6-55B31F3E4C29}

[Control Panel\Desktop]
Wallpaper=%SystemRoot%\Web\Wallpaper\Windows12\windows12.jpg
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

[Control Panel\Cursors]
AppStarting=%SystemRoot%\Resources\Themes\Windows12\Cursors\working.ani
Arrow=%SystemRoot%\Resources\Themes\Windows12\Cursors\arrow.cur
Crosshair=%SystemRoot%\Resources\Themes\Windows12\Cursors\cross.cur
Hand=%SystemRoot%\Resources\Themes\Windows12\Cursors\hand.cur
Help=%SystemRoot%\Resources\Themes\Windows12\Cursors\help.cur
IBeam=%SystemRoot%\Resources\Themes\Windows12\Cursors\beam.cur
No=%SystemRoot%\Resources\Themes\Windows12\Cursors\no.cur
NWPen=%SystemRoot%\Resources\Themes\Windows12\Cursors\pen.cur
SizeAll=%SystemRoot%\Resources\Themes\Windows12\Cursors\move.cur
SizeNESW=%SystemRoot%\Resources\Themes\Windows12\Cursors\nesw.cur
SizeNS=%SystemRoot%\Resources\Themes\Windows12\Cursors\ns.cur
SizeNWSE=%SystemRoot%\Resources\Themes\Windows12\Cursors\nwse.cur
SizeWE=%SystemRoot%\Resources\Themes\Windows12\Cursors\we.cur
UpArrow=%SystemRoot%\Resources\Themes\Windows12\Cursors\up.cur
Wait=%SystemRoot%\Resources\Themes\Windows12\Cursors\busy.ani
"@

$themePath = Join-Path $themeDir "Windows12.theme"
Set-Content -Path $themePath -Value $themeContent

# Create Windows 12 default wallpaper placeholder
$wallpaperContent = @"
This is a placeholder for the Windows 12 wallpaper.
In a real implementation, this would be a .jpg file.
"@

$wallpaperPath = Join-Path $wallpapersDir "windows12.jpg"
Set-Content -Path $wallpaperPath -Value $wallpaperContent

# Create Windows 12 cursor placeholders
$cursorFiles = @("arrow.cur", "beam.cur", "busy.ani", "cross.cur", "hand.cur", "help.cur", "move.cur", "nesw.cur", "no.cur", "ns.cur", "nwse.cur", "pen.cur", "up.cur", "we.cur", "working.ani")
foreach ($cursorFile in $cursorFiles) {
    $cursorPath = Join-Path $cursorsDir $cursorFile
    Set-Content -Path $cursorPath -Value "Windows 12 cursor placeholder for $cursorFile"
}

# Create Windows 12 Start Menu layout
$startLayoutContent = @"
<?xml version="1.0" encoding="utf-8"?>
<LayoutModificationTemplate
    xmlns="http://schemas.microsoft.com/Start/2014/LayoutModification"
    xmlns:defaultlayout="http://schemas.microsoft.com/Start/2014/FullDefaultLayout"
    xmlns:start="http://schemas.microsoft.com/Start/2014/StartLayout"
    xmlns:taskbar="http://schemas.microsoft.com/Start/2014/TaskbarLayout"
    Version="1">
  <LayoutOptions StartTileGroupCellWidth="6" />
  <DefaultLayoutOverride>
    <StartLayoutCollection>
      <defaultlayout:StartLayout GroupCellWidth="6">
        <start:Group Name="Windows 12">
          <start:Tile Size="2x2" Column="0" Row="0" AppUserModelID="Microsoft.WindowsStore_8wekyb3d8bbwe!App"/>
          <start:Tile Size="2x2" Column="2" Row="0" AppUserModelID="Microsoft.MicrosoftEdge_8wekyb3d8bbwe!MicrosoftEdge"/>
          <start:Tile Size="2x2" Column="4" Row="0" AppUserModelID="Microsoft.Windows.Photos_8wekyb3d8bbwe!App"/>
          <start:Tile Size="2x2" Column="0" Row="2" AppUserModelID="Microsoft.WindowsCalculator_8wekyb3d8bbwe!App"/>
          <start:Tile Size="2x2" Column="2" Row="2" AppUserModelID="Microsoft.WindowsAlarms_8wekyb3d8bbwe!App"/>
          <start:Tile Size="2x2" Column="4" Row="2" AppUserModelID="Microsoft.WindowsMaps_8wekyb3d8bbwe!App"/>
        </start:Group>
      </defaultlayout:StartLayout>
    </StartLayoutCollection>
  </DefaultLayoutOverride>
  <CustomTaskbarLayoutCollection>
    <defaultlayout:TaskbarLayout>
      <taskbar:TaskbarPinList>
        <taskbar:UWA AppUserModelID="Microsoft.MicrosoftEdge_8wekyb3d8bbwe!MicrosoftEdge" />
        <taskbar:DesktopApp DesktopApplicationLinkPath="%APPDATA%\Microsoft\Windows\Start Menu\Programs\System Tools\File Explorer.lnk" />
        <taskbar:DesktopApp DesktopApplicationLinkPath="%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\Accessories\Snipping Tool.lnk" />
      </taskbar:TaskbarPinList>
    </defaultlayout:TaskbarLayout>
  </CustomTaskbarLayoutCollection>
</LayoutModificationTemplate>
"@

$startLayoutDir = Join-Path $WindowsPath "Windows\System32\Windows12\StartLayout"
New-Item -Path $startLayoutDir -ItemType Directory -Force | Out-Null
$startLayoutPath = Join-Path $startLayoutDir "Windows12StartLayout.xml"
Set-Content -Path $startLayoutPath -Value $startLayoutContent

# Create Windows 12 OEM info
$oemInfoContent = @"
[General]
Manufacturer=Windows 12 Concept
Model=Windows on ARM
[Support Information]
Line1=Windows 12 Concept UI
Line2=Deployed with WOA-Deployer-Lumia
Line3=https://github.com/maxregnerisch/WOA-Deployer-Lumia
"@

$oemInfoDir = Join-Path $WindowsPath "Windows\System32\oeminfo"
New-Item -Path $oemInfoDir -ItemType Directory -Force | Out-Null
$oemInfoPath = Join-Path $oemInfoDir "oeminfo.ini"
Set-Content -Path $oemInfoPath -Value $oemInfoContent

# Apply Start Layout
$layoutPath = Join-Path $WindowsPath "Windows\System32\Windows12\StartLayout\Windows12StartLayout.xml"
$defaultLayoutPath = Join-Path $WindowsPath "Users\Default\AppData\Local\Microsoft\Windows\Shell"
New-Item -Path $defaultLayoutPath -ItemType Directory -Force | Out-Null
Copy-Item -Path $layoutPath -Destination (Join-Path $defaultLayoutPath "LayoutModification.xml") -Force

Write-Output "Windows 12 Concept UI theme has been applied successfully!"

