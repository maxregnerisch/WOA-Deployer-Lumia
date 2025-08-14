using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Deployer.Services.Wim;
using Deployer.Tasks;
using Serilog;

namespace Deployer.Lumia
{
    public class WoaDeployer : IWoaDeployer
    {
        private readonly IScriptRunner scriptRunner;
        private readonly IScriptParser parser;
        private readonly ITooling tooling;
        private IDeploymentContext context;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IOperationContext operationContext;
        private static readonly string BootstrapPath = Path.Combine("Core", "Bootstrap.txt");

        private static readonly string ScriptsDownloadPath = Path.Combine(AppPaths.ArtifactDownload, "Deployment-Scripts");
        private static readonly string ScriptsBasePath = Path.Combine(ScriptsDownloadPath, "Lumia");

        public WoaDeployer(IScriptRunner scriptRunner, IScriptParser parser, ITooling tooling,
            IFileSystemOperations fileSystemOperations, IOperationContext operationContext)
        {
            this.scriptRunner = scriptRunner;
            this.parser = parser;
            this.tooling = tooling;
            this.fileSystemOperations = fileSystemOperations;
            this.operationContext = operationContext;
        }

        private IPhone Phone => (IPhone)context.Device;

        public async Task Deploy(IDeploymentContext deploymentContext)
        {
            context = deploymentContext;
            operationContext.Start();
            await EnsureFullyUnlocked();

            await DownloadDeploymentScripts();
            await RunDeploymentScript();
            await PatchBootManagerIfNeeded();
            await MoveMetadataToPhone();
            await PreparePhoneDiskForSafeRemoval();
        }

        private async Task DownloadDeploymentScripts()
        {
            if (fileSystemOperations.DirectoryExists(ScriptsDownloadPath))
            {
                await fileSystemOperations.DeleteDirectory(ScriptsDownloadPath);
            }

            await RunScript(BootstrapPath);
        }

        private async Task RunDeploymentScript()
        {
            var dict = new Dictionary<(PhoneModel, Variant), string>
            {
                {(PhoneModel.Talkman, Variant.SingleSim), Path.Combine(ScriptsBasePath, "Talkman", "SingleSim.txt")},
                {(PhoneModel.Cityman, Variant.SingleSim), Path.Combine(ScriptsBasePath, "Cityman", "SingleSim.txt")},
                {(PhoneModel.Talkman, Variant.DualSim), Path.Combine(ScriptsBasePath, "Talkman", "DualSim.txt")},
                {(PhoneModel.Cityman, Variant.DualSim), Path.Combine(ScriptsBasePath, "Cityman", "DualSim.txt")},
            };

            var phoneModel = await Phone.GetModel();
            Log.Verbose("{Model} detected", phoneModel);
            var path = dict[(phoneModel.Model, phoneModel.Variant)];

            await RunScript(path);
            
            var options = context.DeploymentOptions as IDeploymentOptions as WindowsDeploymentOptions;
            if (options != null)
            {
                if (options.ApplyMrosUI)
                {
                    await ApplyMrosUI();
                }
                
                if (options.ApplyWindows12UI)
                {
                    await ApplyWindows12UI();
                }
                
                if (options.Allow24H2On905With3GbRam)
                {
                    await Enable24H2On905With3GbRam();
                }
            }
        }
        
        private async Task ApplyMrosUI()
        {
            Log.Information("Applying MROS UI customizations...");
            try
            {
                var windowsVolume = await context.Device.GetWindowsPartition();
                var systemRoot = Path.Combine(windowsVolume.Root, "Windows");
                
                // Create MROS UI customization script
                var scriptPath = Path.Combine(AppPaths.ArtifactDownload, "ApplyMrosUI.cmd");
                var scriptContent = @"@echo off
echo Applying MROS UI customizations...
reg load HKLM\MROS_SOFTWARE %1\System32\config\SOFTWARE
reg load HKLM\MROS_SYSTEM %1\System32\config\SYSTEM

rem Apply MROS UI customizations
reg add ""HKLM\MROS_SOFTWARE\Microsoft\Windows\CurrentVersion\Themes"" /v ""InstallTheme"" /t REG_EXPAND_SZ /d ""%1\Resources\Themes\mros.theme"" /f
reg add ""HKLM\MROS_SOFTWARE\Microsoft\Windows\CurrentVersion\ThemeManager"" /v ""ThemeActive"" /t REG_SZ /d ""1"" /f
reg add ""HKLM\MROS_SOFTWARE\Microsoft\Windows\CurrentVersion\ThemeManager"" /v ""DllName"" /t REG_EXPAND_SZ /d ""%1\Resources\Themes\mros.msstyles"" /f

reg unload HKLM\MROS_SOFTWARE
reg unload HKLM\MROS_SYSTEM
echo MROS UI customizations applied successfully.
";
                await fileSystemOperations.WriteAllTextToFile(scriptPath, scriptContent);
                
                // Execute the script
                await scriptRunner.Run(parser.Parse($"run-external \"{scriptPath}\" \"{systemRoot}\""));
                
                Log.Information("MROS UI customizations applied successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to apply MROS UI customizations");
            }
        }
        
        private async Task ApplyWindows12UI()
        {
            Log.Information("Applying Windows 12 UI customizations...");
            try
            {
                var windowsVolume = await context.Device.GetWindowsPartition();
                var systemRoot = Path.Combine(windowsVolume.Root, "Windows");
                
                // Create Windows 12 UI customization script
                var scriptPath = Path.Combine(AppPaths.ArtifactDownload, "ApplyWindows12UI.cmd");
                var scriptContent = @"@echo off
echo Applying Windows 12 UI customizations...
reg load HKLM\WIN12_SOFTWARE %1\System32\config\SOFTWARE
reg load HKLM\WIN12_SYSTEM %1\System32\config\SYSTEM

rem Apply Windows 12 UI customizations
reg add ""HKLM\WIN12_SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced"" /v ""TaskbarAl"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\WIN12_SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced"" /v ""TaskbarMn"" /t REG_DWORD /d ""0"" /f
reg add ""HKLM\WIN12_SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced"" /v ""ShowTaskViewButton"" /t REG_DWORD /d ""0"" /f
reg add ""HKLM\WIN12_SOFTWARE\Microsoft\Windows\CurrentVersion\Search"" /v ""SearchboxTaskbarMode"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\WIN12_SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced"" /v ""Start_Layout"" /t REG_DWORD /d ""1"" /f

reg unload HKLM\WIN12_SOFTWARE
reg unload HKLM\WIN12_SYSTEM
echo Windows 12 UI customizations applied successfully.
";
                await fileSystemOperations.WriteAllTextToFile(scriptPath, scriptContent);
                
                // Execute the script
                await scriptRunner.Run(parser.Parse($"run-external \"{scriptPath}\" \"{systemRoot}\""));
                
                Log.Information("Windows 12 UI customizations applied successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to apply Windows 12 UI customizations");
            }
        }
        
        private async Task Enable24H2On905With3GbRam()
        {
            Log.Information("Enabling Windows 11 24H2 compatibility for Lumia 950 with 3GB RAM...");
            try
            {
                var windowsVolume = await context.Device.GetWindowsPartition();
                var systemRoot = Path.Combine(windowsVolume.Root, "Windows");
                
                // Create compatibility script
                var scriptPath = Path.Combine(AppPaths.ArtifactDownload, "Enable24H2Compatibility.cmd");
                var scriptContent = @"@echo off
echo Enabling Windows 11 24H2 compatibility for Lumia 950 with 3GB RAM...
reg load HKLM\COMPAT_SOFTWARE %1\System32\config\SOFTWARE
reg load HKLM\COMPAT_SYSTEM %1\System32\config\SYSTEM

rem Bypass RAM requirements for Windows 11 24H2
reg add ""HKLM\COMPAT_SYSTEM\Setup\MoSetup"" /v ""AllowUpgradesWithUnsupportedTPMOrCPU"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\COMPAT_SYSTEM\Setup\LabConfig"" /v ""BypassRAMCheck"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\COMPAT_SYSTEM\Setup\LabConfig"" /v ""BypassTPMCheck"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\COMPAT_SYSTEM\Setup\LabConfig"" /v ""BypassSecureBootCheck"" /t REG_DWORD /d ""1"" /f
reg add ""HKLM\COMPAT_SYSTEM\Setup\LabConfig"" /v ""BypassCPUCheck"" /t REG_DWORD /d ""1"" /f

rem Optimize for 3GB RAM
reg add ""HKLM\COMPAT_SYSTEM\ControlSet001\Control\Session Manager\Memory Management"" /v ""FeatureSettingsOverride"" /t REG_DWORD /d ""3"" /f
reg add ""HKLM\COMPAT_SYSTEM\ControlSet001\Control\Session Manager\Memory Management"" /v ""FeatureSettingsOverrideMask"" /t REG_DWORD /d ""3"" /f
reg add ""HKLM\COMPAT_SOFTWARE\Microsoft\Windows NT\CurrentVersion\Virtualization"" /v ""MinVmVersionForCpuBasedMitigations"" /t REG_SZ /d ""1.0"" /f

reg unload HKLM\COMPAT_SOFTWARE
reg unload HKLM\COMPAT_SYSTEM
echo Windows 11 24H2 compatibility enabled successfully.
";
                await fileSystemOperations.WriteAllTextToFile(scriptPath, scriptContent);
                
                // Execute the script
                await scriptRunner.Run(parser.Parse($"run-external \"{scriptPath}\" \"{systemRoot}\""));
                
                Log.Information("Windows 11 24H2 compatibility enabled successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to enable Windows 11 24H2 compatibility");
            }
        }

        private async Task RunScript(string path)
        {
            await scriptRunner.Run(parser.Parse(File.ReadAllText(path)));
        }

        private async Task EnsureFullyUnlocked()
        {
            var backUpEfiEsp = await context.Device.GetPartitionByName(PartitionName.BackupEfiesp);
            if (backUpEfiEsp != null)
            {
                throw new InvalidOperationException("Your phone isn't fully unlocked! Please, return to WPInternals and complete the unlock process.");
            }
        }

        private async Task MoveMetadataToPhone()
        {
            try
            {
                var windowsVolume = await context.Device.GetWindowsPartition();
                var destination = Path.Combine(windowsVolume.Root, "Windows", "Logs", "WOA-Deployer");
                await fileSystemOperations.CopyDirectory(AppPaths.Metadata, destination);
                await fileSystemOperations.DeleteDirectory(Path.Combine(AppPaths.Metadata, "Injected Drivers"));
            }
            catch (Exception e)
            {
                Log.Error(e,"Cannot write metadata");
            }
        }

        private async Task PatchBootManagerIfNeeded()
        {
            Log.Debug("Checking if we have to patch WOA's Boot Manager");
            var options = context.DeploymentOptions;
            using (var file = File.OpenRead(options.ImagePath))
            {
                var imageReader = new WindowsImageMetadataReader();
                var windowsImageInfo = imageReader.Load(file);

                var selectedImage = options.ImageIndex - 1;
                if (int.TryParse(windowsImageInfo.Images[selectedImage].Build, out var buildNumber))
                {
                    if (buildNumber == 17763)
                    {
                        Log.Verbose("Build 17763 detected. Patching Boot Manager.");
                        var dest = Path.Combine((await Phone.GetSystemPartition()).Root, "EFI", "Boot") + Path.DirectorySeparatorChar;
                        await fileSystemOperations.Copy(@"Core\Boot\bootaa64.efi", dest);
                        Log.Verbose("Boot Manager Patched.");
                    }
                }
            }
        }

        private async Task PreparePhoneDiskForSafeRemoval()
        {
            Log.Information("# Preparing phone for safe removal");
            Log.Information("Please wait...");
            var disk = await Phone.GetDeviceDisk();
            await disk.PrepareForRemoval();
        }

        public Task ToggleDualBoot(bool isEnabled)
        {
            return tooling.ToogleDualBoot(isEnabled);
        }
    }
}
