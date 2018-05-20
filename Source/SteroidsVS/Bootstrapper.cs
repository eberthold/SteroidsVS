using System;
using Microsoft.VisualStudio.Shell;
using Steroids.CodeQuality.Services;
using Steroids.Contracts;
using Steroids.Contracts.Core;
using SteroidsVS.CodeAdornments;
using SteroidsVS.Services;
using Unity;
using Unity.Lifetime;

namespace SteroidsVS
{
    public class Bootstrapper : IServiceProvider, IBootstrapper
    {
        protected static IUnityContainer RootContainer { get; private set; }

        protected virtual IUnityContainer Container { get; set; }

        public void Run(SteroidsVsPackage package)
        {
            RootContainer = new UnityContainer();
            Container = RootContainer;

            Container.RegisterInstance<Package>(package);
            Container.RegisterInstance(package.VsServiceProvider.ErrorList);
            Container.RegisterInstance(package.VsServiceProvider.OutliningManagerService);
            Container.RegisterInstance(package.VsServiceProvider.ComponentModel);
            Container.RegisterInstance(package.VsServiceProvider.EditorAdapterFactory);
            Container.RegisterInstance(new ActiveTextViewProvider(package.VsServiceProvider.VsTextManager, package.VsServiceProvider.EditorAdapterFactory));

            Container.RegisterType<IDiagnosticProvider, ErrorListDiagnosticProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IActiveTextViewProvider, ActiveTextViewProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDispatcherService, DispatcherService>(new ContainerControlledLifetimeManager());

            Container.RegisterType<CodeStructureOpenCommand>(new ContainerControlledLifetimeManager());
            Container.Resolve<CodeStructureOpenCommand>();
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }
    }
}
