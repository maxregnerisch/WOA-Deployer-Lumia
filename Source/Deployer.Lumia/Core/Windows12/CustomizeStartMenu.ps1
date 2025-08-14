# Windows 12 Start Menu Customization Script
# This script customizes the Start Menu for Windows 12 concept UI

param (
    [Parameter(Mandatory=$false)]
    [string]$WindowsPath = "C:\"
)

Write-Output "Customizing Start Menu for Windows 12 concept UI"

# Registry modifications for Start Menu
$registryPath = Join-Path $WindowsPath "Windows\System32\config\SOFTWARE"
$tempHive = "HKLM\Windows12TempStartMenu"

# Load the SOFTWARE hive
reg load $tempHive $registryPath

# Start Menu layout and appearance
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Start_Layout" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Start_TrackDocs" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "StartMFUEnabled" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Start_ShowClassicMode" /t REG_DWORD /d 0 /f

# Start Menu personalization
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "AppsUseLightTheme" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "SystemUsesLightTheme" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "EnableTransparency" /t REG_DWORD /d 1 /f

# Windows 12 specific Start Menu settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Windows12StartMenu" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Windows12StartMenuTransparency" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Windows12StartMenuAnimation" /t REG_DWORD /d 1 /f

# Unload the hive
reg unload $tempHive

# Create Windows 12 Start Menu layout
$startMenuLayoutContent = @"
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
        <start:Group Name="Productivity">
          <start:Tile Size="2x2" Column="0" Row="0" AppUserModelID="Microsoft.Office.Word_8wekyb3d8bbwe!microsoft.word"/>
          <start:Tile Size="2x2" Column="2" Row="0" AppUserModelID="Microsoft.Office.Excel_8wekyb3d8bbwe!microsoft.excel"/>
          <start:Tile Size="2x2" Column="4" Row="0" AppUserModelID="Microsoft.Office.PowerPoint_8wekyb3d8bbwe!microsoft.powerpoint"/>
          <start:Tile Size="2x2" Column="0" Row="2" AppUserModelID="Microsoft.Office.OneNote_8wekyb3d8bbwe!microsoft.onenote"/>
          <start:Tile Size="2x2" Column="2" Row="2" AppUserModelID="Microsoft.Office.Outlook_8wekyb3d8bbwe!microsoft.outlook"/>
          <start:Tile Size="2x2" Column="4" Row="2" AppUserModelID="Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe!App"/>
        </start:Group>
      </defaultlayout:StartLayout>
    </StartLayoutCollection>
  </DefaultLayoutOverride>
</LayoutModificationTemplate>
"@

$startMenuLayoutDir = Join-Path $WindowsPath "Windows\System32\Windows12\StartMenuLayout"
New-Item -Path $startMenuLayoutDir -ItemType Directory -Force | Out-Null
$startMenuLayoutPath = Join-Path $startMenuLayoutDir "Windows12StartMenuLayout.xml"
Set-Content -Path $startMenuLayoutPath -Value $startMenuLayoutContent

# Apply Start Menu layout
$layoutPath = Join-Path $WindowsPath "Windows\System32\Windows12\StartMenuLayout\Windows12StartMenuLayout.xml"
$defaultLayoutPath = Join-Path $WindowsPath "Users\Default\AppData\Local\Microsoft\Windows\Shell"
New-Item -Path $defaultLayoutPath -ItemType Directory -Force | Out-Null
Copy-Item -Path $layoutPath -Destination (Join-Path $defaultLayoutPath "StartLayoutModification.xml") -Force

Write-Output "Start Menu customization for Windows 12 concept UI completed successfully!"

