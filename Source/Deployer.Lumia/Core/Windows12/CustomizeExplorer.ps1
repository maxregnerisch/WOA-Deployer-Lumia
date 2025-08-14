# Windows 12 Explorer Customization Script
# This script customizes File Explorer for Windows 12 concept UI

param (
    [Parameter(Mandatory=$false)]
    [string]$WindowsPath = "C:\"
)

Write-Output "Customizing File Explorer for Windows 12 concept UI"

# Registry modifications for Explorer
$registryPath = Join-Path $WindowsPath "Windows\System32\config\SOFTWARE"
$tempHive = "HKLM\Windows12TempExplorer"

# Load the SOFTWARE hive
reg load $tempHive $registryPath

# Explorer view settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "LaunchTo" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "NavPaneShowAllFolders" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "NavPaneExpandToCurrentFolder" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "SeparateProcess" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "FolderContentsInfoTip" /t REG_DWORD /d 1 /f

# Explorer ribbon settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Ribbon" /v "MinimizedStateTabletModeOff" /t REG_DWORD /d 0 /f

# Explorer search settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Search\Preferences" /v "WholeFileSystem" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Search\Preferences" /v "SystemFolders" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Search\Preferences" /v "ArchivedFiles" /t REG_DWORD /d 1 /f

# Explorer file operations settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\OperationStatusManager" /v "ConfirmationCheckBoxDoForAll" /t REG_DWORD /d 1 /f

# Explorer context menu settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ShowInfoTip" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ShowCompColor" /t REG_DWORD /d 1 /f

# Windows 12 Explorer theme settings
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "EnableTransparency" /t REG_DWORD /d 1 /f
reg add "$tempHive\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "ColorPrevalence" /t REG_DWORD /d 1 /f

# Unload the hive
reg unload $tempHive

Write-Output "File Explorer customization for Windows 12 concept UI completed successfully!"

