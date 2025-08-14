# Windows 12 Concept UI for WOA-Deployer-Lumia

This directory contains scripts and resources for applying Windows 12 concept UI to Windows installations deployed with WOA-Deployer-Lumia.

## Features

- Complete Windows 12 concept UI theme
- Registry modifications for Windows 12 appearance
- Custom Start Menu layout
- Custom Taskbar configuration
- Explorer customizations
- OEM information customization
- Wallpaper and cursor placeholders
- Support for 3GB RAM allocation

## Implementation Details

### Main Components

1. **ApplyWindows12Theme.ps1**
   - Creates theme directories
   - Modifies registry settings
   - Creates theme files
   - Sets up wallpapers and cursors

2. **CustomizeExplorer.ps1**
   - Customizes File Explorer for Windows 12 look and feel
   - Modifies Explorer registry settings

3. **CustomizeTaskbar.ps1**
   - Configures taskbar appearance and behavior
   - Sets up taskbar layout

4. **CustomizeStartMenu.ps1**
   - Configures Start Menu appearance and behavior
   - Sets up Start Menu layout

5. **Windows12UI.txt**
   - Main deployment script that orchestrates the Windows 12 UI application
   - Called by WoaDeployer.cs after Windows deployment

## How It Works

During Windows deployment, after the standard deployment process completes, the WoaDeployer calls the Windows12UI.txt script, which:

1. Creates necessary directories
2. Copies Windows 12 resources to the Windows installation
3. Runs the PowerShell scripts to apply the Windows 12 theme and customizations

This results in a Windows installation with a Windows 12 concept UI look and feel.

## Customization

You can modify the scripts and resources in this directory to customize the Windows 12 concept UI according to your preferences.

## Requirements

- Windows on ARM installation
- Minimum 3GB RAM allocation (configurable in the UI)

