using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Lumia.Core;
using Deployer.Tasks;

namespace Deployer.Lumia
{
    public class WindowsDeploymentOptionsProvider : IWindowsOptionsProvider
    {
        private readonly IPhone phone;
        private readonly IEnumerable<IWindowsDeploymentOption> options;

        public WindowsDeploymentOptionsProvider(IPhone phone, IEnumerable<IWindowsDeploymentOption> options)
        {
            this.phone = phone ?? throw new ArgumentNullException(nameof(phone));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<WindowsDeploymentOptions> GetWindowsDeploymentOptions()
        {
            var isLumia950 = phone.Name.Contains("Lumia 950");
            var isLumia950XL = phone.Name.Contains("Lumia 950 XL");
            var is24H2Compatible = isLumia950 && phone.Memory >= 3 * 1024; // 3GB RAM for Lumia 950

            var availableOptions = options.ToList();

            var result = new WindowsDeploymentOptions
            {
                SupportedOptions = availableOptions,
                RecommendedOption = availableOptions.First(),
                DualBootEnabled = await phone.GetDualBootStatus(),
                IsWoaPresent = await phone.IsWoAPresent(),
                Windows11Compatible = isLumia950 || isLumia950XL,
                Windows11_24H2Compatible = is24H2Compatible
            };

            return result;
        }
    }

    public class WindowsDeploymentOptions
    {
        public IEnumerable<IWindowsDeploymentOption> SupportedOptions { get; set; }
        public IWindowsDeploymentOption RecommendedOption { get; set; }
        public bool DualBootEnabled { get; set; }
        public bool IsWoaPresent { get; set; }
        public bool Windows11Compatible { get; set; }
        public bool Windows11_24H2Compatible { get; set; }
    }
}

