using Steroids.Core.CodeQuality;
using Steroids.Core.Editor;
using Steroids.Core.Framework;
using SteroidsVS.CodeAdornments;
using SteroidsVS.CodeQuality.Diagnostic;
using SteroidsVS.Editor;
using SteroidsVS.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace SteroidsVS
{
    public class Bootstrapper : IServiceProvider, IBootstrapper
    {
        private static TaskCompletionSource<IUnityContainer> _rootContainerTcs = new TaskCompletionSource<IUnityContainer>();

        protected static IUnityContainer RootContainer { get; private set; }

        protected virtual IUnityContainer Container { get; set; }

        public virtual void Run(IVsServiceProvider vsServiceProvider)
        {
            RootContainer = new UnityContainer();
            Container = RootContainer;

            Container.RegisterInstance(vsServiceProvider);
            Container.RegisterInstance(vsServiceProvider.ErrorList);
            Container.RegisterInstance(vsServiceProvider.OutliningManagerService);
            Container.RegisterInstance(vsServiceProvider.ComponentModel);
            Container.RegisterInstance(vsServiceProvider.EditorAdapterFactory);
            Container.RegisterInstance(vsServiceProvider.TableManagerProvider);
            Container.RegisterInstance(new ActiveTextViewProvider(vsServiceProvider.VsTextManager, vsServiceProvider.EditorAdapterFactory));

            Container.RegisterSingleton<IDiagnosticProvider, ErrorListDiagnosticProvider>();
            Container.RegisterSingleton<IDispatcherServiceFactory, DispatcherServiceFactory>();
            Container.RegisterSingleton<ICancellationServiceFactory, CancellationServiceFactory>();

            Container.RegisterType<IActiveTextViewProvider, ActiveTextViewProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IFoldingManager, FoldingManagerAdapter>(new ContainerControlledLifetimeManager());

            Container.RegisterType<CodeStructureOpenCommand>(new ContainerControlledLifetimeManager());

            Container.RegisterType<CodeStructureOpenCommand>(new ContainerControlledLifetimeManager());
            Task.Run(() =>
            {
                Container.Resolve<CodeStructureOpenCommand>();
            });

            _rootContainerTcs.SetResult(RootContainer);
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            if (Container is null)
            {
                Debug.WriteLine($"Container was not initialized while resolving {serviceType.Name}");
                return null;
            }

            return Container.Resolve(serviceType);
        }

        protected static Task<IUnityContainer> GetRootContainerAsync()
        {
            return _rootContainerTcs.Task;
        }
    }
}
