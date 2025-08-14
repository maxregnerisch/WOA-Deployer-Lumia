﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Deployer.DevOpsBuildClient;
using Deployer.Execution;
using Deployer.FileSystem;
using Deployer.Lumia.NetFx.PhoneMetadata;
using Deployer.NetFx;
using Deployer.Services;
using Deployer.Tasks;
using Grace.DependencyInjection;
using Octokit;
using Superpower;

namespace Deployer.Lumia.NetFx
{
    public static class ContainerConfigurator
    {
        public static IExportRegistrationBlock ExportCommon(this IExportRegistrationBlock block)
        {
            var taskTypes = from a in Assemblies.AppDomainAssemblies
                            from type in a.ExportedTypes
                            where type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeploymentTask))
                            select type;
            block.ExportAssemblies(Assemblies.AppDomainAssemblies).ByInterface<ISpaceAllocator<IPhone>>().Lifestyle.Singleton();
            block.ExportAssemblies(Assemblies.AppDomainAssemblies).BasedOn<LumiaDiskLayoutPreparer>().ByInterface<IDiskLayoutPreparer>().ExportAttributedTypes().Lifestyle.Singleton();
            block.Export<ZipExtractor>().As<IZipExtractor>();
            block.ExportFactory(Tokenizer.Create).As<Tokenizer<LangToken>>().Lifestyle.Singleton();
            block.Export<ScriptParser>().As<IScriptParser>().Lifestyle.Singleton();
            block.Export<PhoneInfoReader>().As<IPhoneInfoReader>().Lifestyle.Singleton();
            block.Export<WoaDeployer>().As<IWoaDeployer>().Lifestyle.Singleton();
            block.Export<Tooling>().As<ITooling>().Lifestyle.Singleton();
            block.Export<BootCreator>().As<IBootCreator>().Lifestyle.Singleton();
            block.Export<PhonePathBuilder>().As<IPathBuilder>().Lifestyle.Singleton();
            block.ExportInstance(taskTypes).As<IEnumerable<Type>>();
            block.Export<ScriptRunner>().As<IScriptRunner>().Lifestyle.Singleton();
            block.Export<InstanceBuilder>().As<IInstanceBuilder>().Lifestyle.Singleton();
            block.Export<FileSystemOperations>().As<IFileSystemOperations>().Lifestyle.Singleton();
            block.Export<BcdInvokerFactory>().As<IBcdInvokerFactory>().Lifestyle.Singleton();
            block.Export<WindowsDeployer>().As<IWindowsDeployer>().Lifestyle.Singleton();
            block.ExportFactory(() => new HttpClient { Timeout = TimeSpan.FromMinutes(30) }).Lifestyle.Singleton();
            block.ExportFactory(() => new GitHubClient(new ProductHeaderValue("WOADeployer"))).As<IGitHubClient>().Lifestyle.Singleton();
            block.Export<Downloader>().As<IDownloader>().Lifestyle.Singleton();
            block.Export<ExistingDeploymentCleaner>().As<IExistingDeploymentCleaner>().Lifestyle.Singleton();
            block.ExportFactory((IPhone phone) => new DeploymentContext { Device = phone }).As<IDeploymentContext>().Lifestyle.Singleton();
            block.Export<OperationContext>().As<IOperationContext>().Lifestyle.Singleton();
            block.ExportFactory(() => AzureDevOpsBuildClient.Create(new Uri("https://dev.azure.com"))).As<IAzureDevOpsBuildClient>().Lifestyle.Singleton();
            block.Export<PhoneModelInfoInfoReader>().As<IPhoneModelInfoReader>().Lifestyle.Singleton();
            block.Export<Phone>().As<IPhone>().Lifestyle.Singleton();
            block.Export<DismImageService>().As<IWindowsImageService>().Lifestyle.Singleton();
            block.Export<LogCollector>().As<ILogCollector>().Lifestyle.Singleton();
            block.Export<DiskRoot>().As<IDiskRoot>().Lifestyle.Singleton();

            return block;

        }
    }
}