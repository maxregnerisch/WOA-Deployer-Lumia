using System.Threading.Tasks;
using Deployer.Lumia.NetFx;
using Deployer.NetFx;
using Xunit;

namespace Deployer.Lumia.Tests
{
    public class LumiaDiskLayoutPreparerTests
    {
        [Fact(Skip = "Don't run this")]
        [Trait("Category", "Real")]
        public async Task Test()
        {
            //var api = new DiskApi();
            //var operations = new DiskFilesystemOperations(api);
            //var allocators = new SpaceAllocators();

            //var optionsProvider = new WindowsDeploymentOptionsProvider
            //{
            //    Options = new WindowsDeploymentOptions
            //    {
            //        SizeReservedForWindows = 200, // MB
            //    }
            //};

            //var phone = new TestPhone(api, null, null);
            //var preparer = new LumiaDiskLayoutPreparer(optionsProvider, operations, allocators, new PartitionCleaner(), phone);

            //var disk = await api.GetDisk(3);
            //await preparer.Prepare(disk);
        }       
    }
}

