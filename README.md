# WOA-Deployer-Lumia

[![Build WOA-Deployer-Lumia](https://github.com/maxregnerisch/WOA-Deployer-Lumia/actions/workflows/build.yml/badge.svg)](https://github.com/maxregnerisch/WOA-Deployer-Lumia/actions/workflows/build.yml)

Deploy Windows 10/11 ARM64 to your Lumia 950/950 XL with ease!

## Features

- Deploy Windows 10/11 ARM64 to Lumia 950/950 XL
- Apply MROS UI customizations
- Apply Windows 12 UI customizations
- Enable Windows 11 24H2 compatibility on Lumia 950 with 3GB RAM
- Dual-boot support
- Compact OS deployment option

## Requirements

- A Lumia 950/950 XL with unlocked bootloader
- A Windows 10/11 ARM64 WIM file
- Windows 10 or 11 PC with .NET Framework 4.7.2 or higher

## Building from Source

### Prerequisites

- Visual Studio 2019 or newer
- .NET Framework 4.7.2 SDK
- .NET Core 3.1 SDK

### Build Instructions

1. Clone the repository with submodules:
   ```
   git clone --recursive https://github.com/maxregnerisch/WOA-Deployer-Lumia.git
   ```

2. Open the solution in Visual Studio:
   ```
   Source/WoaDeployer for Lumia.sln
   ```

3. Restore NuGet packages

4. Build the solution in Release mode

## Command Line Options

The tool supports the following command line options:

```
--wim                   Windows Image (.wim) to deploy
--index                 Index of the image to deploy (default: 1)
--windows-size          Size reserved for Windows partitions in GB (default: 18)
--compact               Enable Compact deployment (slower, but saves phone disk space)
--apply-mros-ui         Apply MROS UI customizations during deployment
--apply-win12-ui        Apply Windows 12 UI customizations during deployment
--allow-24h2-on-905-3gb Enable compatibility for running Windows 11 24H2 on Lumia 950 with 3GB RAM
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- WOA Project team for their amazing work on Windows on ARM
- All contributors to this project

