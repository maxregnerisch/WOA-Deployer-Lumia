using CommandLine;
using Deployer.Console;
using Deployer.Lumia.Console.Options;
using Serilog;
using Serilog.Events;

namespace Deployer.Lumia.Console
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            ConfigureLogger();

            try
            {
                var container = ContainerConfigurator.Create();
                var deployer = container.Locate<LumiaDeployer>();

                var parserResult = Parser.Default
                    .ParseArguments<WindowsDeploymentCmdOptions, EnableDualBootCmdOptions,
                        DisableDualBootCmdOptions, NonWindowsDeploymentCmdOptions>(args);

                var result = await parserResult.MapResult(
                    async (WindowsDeploymentCmdOptions opts) =>
                    {
                        deployer.Options = new WindowsDeploymentOptions
                        {
                            ImagePath = opts.WimImage,
                            ImageIndex = opts.Index,
                            UseCompact = opts.UseCompact,
                            SizeReservedForWindows = opts.ReservedSizeForWindowsInGb,
                            ApplyMrosUI = opts.ApplyMrosUI,
                            ApplyWindows12UI = opts.ApplyWindows12UI,
                            Allow24H2On905With3GbRam = opts.Allow24H2On905With3GbRam,
                        };
                        return deployer.Deploy();
                    },
                    (EnableDualBootCmdOptions opts) => deployer.ToggleDualBoot(true),
                    (DisableDualBootCmdOptions opts) => deployer.ToggleDualBoot(false),
                    (NonWindowsDeploymentCmdOptions opts) => deployer.Deploy(),
                    errors => Task.FromResult(1));

                return result;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Operation failed");
                return 1;
            }
        }

        private static void ConfigureLogger()
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "WOA Deployer.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(LogEventLevel.Information)
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}

