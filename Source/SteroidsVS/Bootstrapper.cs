using System;
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

        public void Run(IVsServiceProvider vsServiceProvider)
        {
            RootContainer = new UnityContainer();
            Container = RootContainer;

            Container.RegisterInstance(vsServiceProvider);
            Container.RegisterInstance(vsServiceProvider.ErrorList);
            Container.RegisterInstance(vsServiceProvider.OutliningManagerService);
            Container.RegisterInstance(vsServiceProvider.ComponentModel);
            Container.RegisterInstance(vsServiceProvider.EditorAdapterFactory);
            Container.RegisterInstance(new ActiveTextViewProvider(vsServiceProvider.VsTextManager, vsServiceProvider.EditorAdapterFactory));

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
