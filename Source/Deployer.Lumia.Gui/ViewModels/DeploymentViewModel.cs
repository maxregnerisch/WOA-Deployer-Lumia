using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Lumia.Gui.Properties;
using Deployer.Tasks;
using Deployer.UI;
using Deployer.UI.ViewModels;
using Grace.DependencyInjection.Attributes;
using ReactiveUI;
using Serilog;

namespace Deployer.Lumia.Gui.ViewModels
{
    [Metadata("Name", "Deployment")]
    [Metadata("Order", 0)]
    public class DeploymentViewModel : ReactiveObject, ISection
    {
        private readonly IDeploymentContext context;
        private readonly IWoaDeployer woaDeployer;
        private readonly UIServices uiServices;
        private readonly AdvancedViewModel advancedViewModel;
        private readonly WimPickViewModel wimPickViewModel;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly ILumiaSettingsService lumiaSettingsService;
        private readonly ObservableAsPropertyHelper<bool> isBusyHelper;

        public DeploymentViewModel(
            IDeploymentContext context,
            IWoaDeployer woaDeployer,
            IOperationContext operationContext, UIServices uiServices, AdvancedViewModel advancedViewModel, IOperationProgress progress,
            WimPickViewModel wimPickViewModel, IFileSystemOperations fileSystemOperations, ILumiaSettingsService lumiaSettingsService)
        {
            this.context = context;
            this.woaDeployer = woaDeployer;
            this.uiServices = uiServices;
            this.advancedViewModel = advancedViewModel;
            this.wimPickViewModel = wimPickViewModel;
            this.fileSystemOperations = fileSystemOperations;
            this.lumiaSettingsService = lumiaSettingsService;

            var isSelectedWim = wimPickViewModel.WhenAnyObservable(x => x.WimMetadata.SelectedImageObs)
                .Select(metadata => metadata != null);

            FullInstallWrapper = new ProgressViewModel(ReactiveCommand.CreateFromTask(Deploy, isSelectedWim), progress, this, uiServices.ContextDialog, operationContext);
            IsBusyObservable = FullInstallWrapper.Command.IsExecuting;
            isBusyHelper = IsBusyObservable.ToProperty(this, model => model.IsBusy);
        }

        public bool IsBusy => isBusyHelper.Value;

        private async Task Deploy()
        {
            Log.Information("# Starting deployment...");

            var windowsDeploymentOptions = new WindowsDeploymentOptions
            {
                ImagePath = wimPickViewModel.WimMetadata.Path,
                ImageIndex = wimPickViewModel.WimMetadata.SelectedDiskImage.Index,
                UseCompact = advancedViewModel.UseCompactDeployment,
            };

            context.DeploymentOptions = windowsDeploymentOptions;
            
            await CleanDownloadedIfNeeded();
            await woaDeployer.Deploy(context);

            Log.Information("Deployment successful");

            await uiServices.Dialog.Pick(Resources.WindowsDeployedSuccessfully, new List<Option>()
            {
                new Option("Close")
            });
        }

        private async Task CleanDownloadedIfNeeded()
        {
            if (!lumiaSettingsService.CleanDownloadedBeforeDeployment)
            {
                return;
            }

            if (fileSystemOperations.DirectoryExists(AppPaths.ArtifactDownload))
            {
                Log.Information("Deleting previously downloaded deployment files");
                await fileSystemOperations.DeleteDirectory(AppPaths.ArtifactDownload);
            }
        }

        public ProgressViewModel FullInstallWrapper { get; set; }
        public IObservable<bool> IsBusyObservable { get; }
    }
}