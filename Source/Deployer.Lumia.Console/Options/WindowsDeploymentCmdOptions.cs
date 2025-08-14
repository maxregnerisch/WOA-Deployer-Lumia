using CommandLine;

namespace Deployer.Lumia.Console.Options
{
    [Verb("deploy", HelpText = "Executes a Windows deployment script")]
    public class WindowsDeploymentCmdOptions
    {
        [Option("wim", Required = true, HelpText = "Windows Image (.wim) to deploy")]
        public string WimImage { get; set; }

        [Option("index", Default = 1, HelpText = "Index of the image to deploy")]
        public int Index { get; set; }

        [Option("windows-size", Default = 18, HelpText = "Size reserved for Windows partitions in GB")]
        public double ReservedSizeForWindowsInGb { get; set; }

        [Option("compact", Default = false, HelpText = "Enable Compact deployment. Slower, but saves phone disk space")]
        public bool UseCompact { get; set; }
        
        [Option("apply-mros-ui", Default = false, HelpText = "Apply MROS UI customizations during deployment")]
        public bool ApplyMrosUI { get; set; }
        
        [Option("apply-win12-ui", Default = false, HelpText = "Apply Windows 12 UI customizations during deployment")]
        public bool ApplyWindows12UI { get; set; }
        
        [Option("allow-24h2-on-905-3gb", Default = false, HelpText = "Enable compatibility for running Windows 11 24H2 on Lumia 950 with 3GB RAM")]
        public bool Allow24H2On905With3GbRam { get; set; }
    }
}

