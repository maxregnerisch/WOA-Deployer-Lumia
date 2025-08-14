using ByteSizeLib;

namespace Deployer.Lumia
{
    public class WindowsDeploymentOptions : DeploymentOptions
    {
        public string ImagePath { get; set; }
        public int ImageIndex { get; set; }
        public ByteSize SizeReservedForWindows { get; set; }
        public bool UseCompact { get; set; }
        public bool ApplyMrosUI { get; set; }
        public bool ApplyWindows12UI { get; set; }
        public bool Allow24H2On905With3GbRam { get; set; }
    }
}

