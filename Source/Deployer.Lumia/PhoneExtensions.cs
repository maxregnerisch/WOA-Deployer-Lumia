using System.Threading.Tasks;
using Deployer.FileSystem;
using Serilog;

namespace Deployer.Lumia
{
    public static class PhoneExtensions
    {
        public static async Task<bool> HasEnoughSpace(this IPhone phone, double requiredSpace)
        {
            Log.Verbose("Checking if there's enough space in the phone...");
            
            var disk = await phone.GetDeviceDisk();
            var data = await disk.GetPartitionByName(PartitionName.Data);
            
            if (data == null)
            {
                Log.Verbose("Data partition not found. Cannot check available space.");
                return false;
            }

            var available = await data.GetAvailableSize();
            var availableGb = available / 1024.0;
            
            Log.Verbose("Available: {Size} GB", availableGb);
            Log.Verbose("Required: {Size} GB", requiredSpace);
            
            return availableGb >= requiredSpace;
        }
    }
}

