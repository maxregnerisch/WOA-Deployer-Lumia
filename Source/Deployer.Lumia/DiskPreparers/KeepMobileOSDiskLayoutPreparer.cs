using Deployer.Exceptions;
using Deployer.Tasks;
using Grace.DependencyInjection.Attributes;
using Serilog;

namespace Deployer.Lumia.DiskPreparers
{
    [Metadata("Name", "Keep Windows 10 Mobile")]
    [Metadata("Order", 0)]
    public class KeepMobileOSDiskLayoutPreparer : LumiaDiskLayoutPreparer
    {
        private readonly IPhone phone;
        private readonly ISpaceAllocator<IPhone> spaceAllocator;
        private readonly ILumiaSettingsService lumiaSettingsService;

        public KeepMobileOSDiskLayoutPreparer(IDeploymentContext context, IExistingDeploymentCleaner cleaner,
            IPhone phone, ISpaceAllocator<IPhone> spaceAllocator, ILumiaSettingsService lumiaSettingsService)
            : base(context, cleaner)
        {
            this.phone = phone;
            this.spaceAllocator = spaceAllocator;
            this.lumiaSettingsService = lumiaSettingsService;
        }

        public double SizeReservedForWindows
        {
            get => lumiaSettingsService.SizeReservedForWindows;
            set
            {
                lumiaSettingsService.SizeReservedForWindows = value;
            }
        }

        private async Task AllocateSpace(double requiredSize)
        {
            Log.Information("Verifying available space");
            Log.Verbose("Verifying the available space...");
            Log.Verbose("We will need {Size} of free space for Windows", requiredSize);

            var hasEnoughSpace = await Phone.HasEnoughSpace(requiredSize);
            if (!hasEnoughSpace)
            {
                Log.Verbose("There's not enough space in the phone. We will try to allocate it automatically");

                var success = await spaceAllocator.TryAllocate(phone, requiredSize);
                if (!success)
                {
                    throw new NotEnoughSpaceException($"Could not allocate {requiredSize} on the phone. Please, try to allocate more space manually.");
                }
            }
        }

        protected override async Task CreatePartitions()
        {
            await AllocateSpace(SizeReservedForWindows);

            var mainOs = await disk.GetPartitionByName(PartitionName.MainOs);
            var data = await disk.GetPartitionByName(PartitionName.Data);

            if (mainOs == null)
            {
                throw new ApplicationException("MainOS partition is null");
            }

            if (data == null)
            {
                throw new ApplicationException("Data partition is null");
            }

            var dataSize = await data.GetSize();
            var mainOsSize = await mainOs.GetSize();

            Log.Verbose("Data size is {Size}", dataSize);
            Log.Verbose("MainOS size is {Size}", mainOsSize);

            var espSize = 100; // MB
            var winSize = SizeReservedForWindows - espSize;

            Log.Verbose("Creating Windows partition of {Size}", winSize);
            var winPart = await disk.CreateGptPartition(winSize);
            await winPart.SetGptType(PartitionType.Basic);
            await winPart.Format(FileSystemFormat.Ntfs, "WindowsARM");
            await winPart.SetPartitionName(PartitionName.Windows);

            Log.Verbose("Creating ESP partition of {Size}", espSize);
            var espPart = await disk.CreateGptPartition(espSize);
            await espPart.SetGptType(PartitionType.Esp);
            await espPart.Format(FileSystemFormat.Fat32, "BOOT");
            await espPart.SetPartitionName(PartitionName.Esp);
        }
    }
}

