using System;
using Steroids.Contracts.Core;
using Steroids.Core.CodeQuality;
using Steroids.Core.Editor;
using Steroids.Core.Framework;
using Steroids.Core.Settings;
using SteroidsVS.CodeAdornments;
using SteroidsVS.CodeQuality.Diagnostic;
using SteroidsVS.Editor;
using SteroidsVS.Services;
using SteroidsVS.Settings;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;

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
            Container.RegisterInstance(vsServiceProvider.TableManagerProvider);
            Container.RegisterInstance(vsServiceProvider.ServiceProvider);
            Container.RegisterInstance(new ActiveTextViewProvider(vsServiceProvider.VsTextManager, vsServiceProvider.EditorAdapterFactory));

            Container.RegisterSingleton<ISettingsController, SettingsController>();
            Container.RegisterSingleton<IEventAggregator, EventAggregator>();
            Container.RegisterSingleton<CodeStructure.Settings.ISettingsService, CodeStructure.Settings.SettingsService>();

            Container.RegisterType<IDiagnosticProvider, ErrorListDiagnosticProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IActiveTextViewProvider, ActiveTextViewProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDispatcherService, DispatcherService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IFoldingManager, FoldingManagerAdapter>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureOpenCommand>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructure.Settings.SettingsViewModel>();

            Container.RegisterType<CodeStructureOpenCommand>(new ContainerControlledLifetimeManager());
            Container.Resolve<CodeStructureOpenCommand>();

            var provider = new UnityServiceLocator(RootContainer);
            CommonServiceLocator.ServiceLocator.SetLocatorProvider(() => provider);
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }
    }
}
