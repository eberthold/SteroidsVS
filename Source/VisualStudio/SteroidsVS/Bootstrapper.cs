using System;
using Microsoft.Extensions.Logging;
using Steroids.Contracts.Core;
using Steroids.Core.CodeQuality;
using Steroids.Core.Editor;
using Steroids.Core.Framework;
using SteroidsVS.CodeAdornments;
using SteroidsVS.CodeQuality.Diagnostic;
using SteroidsVS.Editor;
using SteroidsVS.Logging;
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

            Container.RegisterInstance<ILogger>(new ActivityLogLogger());
            Container.RegisterInstance(vsServiceProvider);
            Container.RegisterInstance(vsServiceProvider.ErrorList);
            Container.RegisterInstance(vsServiceProvider.OutliningManagerService);
            Container.RegisterInstance(vsServiceProvider.ComponentModel);
            Container.RegisterInstance(vsServiceProvider.EditorAdapterFactory);
            Container.RegisterInstance(vsServiceProvider.TableManagerProvider);
            Container.RegisterInstance(new ActiveTextViewProvider(vsServiceProvider.VsTextManager, vsServiceProvider.EditorAdapterFactory));

            Container.RegisterType<IDiagnosticProvider, ErrorListDiagnosticProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IActiveTextViewProvider, ActiveTextViewProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDispatcherService, DispatcherService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IFoldingManager, FoldingManagerAdapter>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureOpenCommand>(new ContainerControlledLifetimeManager());

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
