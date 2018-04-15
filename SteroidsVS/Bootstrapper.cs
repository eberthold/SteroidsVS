using System;
using Microsoft.VisualStudio.Shell;
using Steroids.CodeQuality.Services;
using Steroids.Contracts;
using Steroids.Contracts.Core;
using SteroidsVS.CodeAdornments;
using SteroidsVS.Contracts;
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
            Container.RegisterInstance(package.Workspace);
            Container.RegisterInstance(package.ErrorList);
            Container.RegisterInstance(package.OutliningManagerService);
            Container.RegisterInstance(package.ComponentModel);
            Container.RegisterInstance(package.EditorAdapterFactory);
            Container.RegisterInstance(new ActiveTextViewProvider(package.VsTextManager, package.EditorAdapterFactory));

            Container.RegisterType<IWorkspaceManager, WorkspaceManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDiagnosticProvider, ErrorListDiagnosticProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IActiveTextViewProvider, ActiveTextViewProvider>(new ContainerControlledLifetimeManager());

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
