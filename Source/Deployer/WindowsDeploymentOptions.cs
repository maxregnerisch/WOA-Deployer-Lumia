namespace Deployer
{
    public class WindowsDeploymentOptions
    {
        public string ImagePath { get; set; }
        public int ImageIndex { get; set; }
        public double SizeReservedForWindows { get; set; }
        public bool UseCompact { get; set; }
        public bool ApplyMrosUI { get; set; }
        public bool ApplyWindows12UI { get; set; }
        public bool Allow24H2On905With3GbRam { get; set; }
    }
}

