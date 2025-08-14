# Windows 12 Taskbar Customization Script
# This script customizes the Taskbar for Windows 12 concept UI

param (
    [Parameter(Mandatory=$false)]
    [string]$WindowsPath = "C:\"
)

Write-Output "Customizing Taskbar for Windows 12 concept UI"

# Registry modifications for Taskbar
$registryPath = Join-Path $WindowsPath "Windows\System32\config\SOFTWARE"
$tempHive = "HKLM\Windows12TempTaskbar"

# Load the SOFTWARE hive
reg load $tempHive $registryPath

# Taskbar position and alignment
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarAl" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarSi" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarSmallIcons" /t REG_DWORD /d 0 /f

# Taskbar buttons and features
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ShowTaskViewButton" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarDa" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarMn" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarAnimations" /t REG_DWORD /d 1 /f

# Taskbar appearance
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarAutoHideInTabletMode" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarGlomLevel" /t REG_DWORD /d 0 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "MMTaskbarEnabled" /t REG_DWORD /d 1 /f

# System tray settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer" /v "EnableAutoTray" /t REG_DWORD /d 0 /f

# Windows 12 specific taskbar settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Windows12Taskbar" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Windows12TaskbarTransparency" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Windows12TaskbarAnimation" /t REG_DWORD /d 1 /f

# Unload the hive
reg unload $tempHive

# Create Windows 12 taskbar layout
$taskbarLayoutContent = @"
<?xml version="1.0" encoding="utf-8"?>
<LayoutModificationTemplate
    xmlns="http://schemas.microsoft.com/Start/2014/LayoutModification"
    xmlns:defaultlayout="http://schemas.microsoft.com/Start/2014/FullDefaultLayout"
    xmlns:start="http://schemas.microsoft.com/Start/2014/StartLayout"
    xmlns:taskbar="http://schemas.microsoft.com/Start/2014/TaskbarLayout"
    Version="1">
  <CustomTaskbarLayoutCollection>
    <defaultlayout:TaskbarLayout>
      <taskbar:TaskbarPinList>
        <taskbar:UWA AppUserModelID="Microsoft.MicrosoftEdge_8wekyb3d8bbwe!MicrosoftEdge" />
        <taskbar:DesktopApp DesktopApplicationLinkPath="%APPDATA%\Microsoft\Windows\Start Menu\Programs\System Tools\File Explorer.lnk" />
        <taskbar:DesktopApp DesktopApplicationLinkPath="%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\Accessories\Snipping Tool.lnk" />
        <taskbar:DesktopApp DesktopApplicationLinkPath="%APPDATA%\Microsoft\Windows\Start Menu\Programs\Accessories\Notepad.lnk" />
      </taskbar:TaskbarPinList>
    </defaultlayout:TaskbarLayout>
  </CustomTaskbarLayoutCollection>
</LayoutModificationTemplate>
"@

$taskbarLayoutDir = Join-Path $WindowsPath "Windows\System32\Windows12\TaskbarLayout"
New-Item -Path $taskbarLayoutDir -ItemType Directory -Force | Out-Null
$taskbarLayoutPath = Join-Path $taskbarLayoutDir "Windows12TaskbarLayout.xml"
Set-Content -Path $taskbarLayoutPath -Value $taskbarLayoutContent

# Apply taskbar layout
$layoutPath = Join-Path $WindowsPath "Windows\System32\Windows12\TaskbarLayout\Windows12TaskbarLayout.xml"
$defaultLayoutPath = Join-Path $WindowsPath "Users\Default\AppData\Local\Microsoft\Windows\Shell"
New-Item -Path $defaultLayoutPath -ItemType Directory -Force | Out-Null
Copy-Item -Path $layoutPath -Destination (Join-Path $defaultLayoutPath "TaskbarLayoutModification.xml") -Force

Write-Output "Taskbar customization for Windows 12 concept UI completed successfully!"

