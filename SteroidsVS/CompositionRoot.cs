namespace SteroidsVS
{
    using System;
    using System.ComponentModel.Composition;
    using Steroids.CodeStructure.Analyzers.Services;
    using Steroids.Contracts;
    using SteroidsVS.Contracts;
    using Unity;
    using Unity.Lifetime;

    [Export(typeof(ICompositionRoot))]
    public class CompositionRoot : IServiceProvider, ICompositionRoot
    {
        protected static IUnityContainer RootContainer { get; private set; }

        protected virtual IUnityContainer Container { get; set; }

        public void Initialize(IVsPackageServices services)
        {
            RootContainer = new UnityContainer();
            Container = RootContainer;

            Container.RegisterInstance(services.Workspace);
            Container.RegisterInstance(services.ErrorList);

            Container.RegisterType<IWorkspaceManager, WorkspaceManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDocumentAnalyzerService, DocumentAnalyzerService>(new HierarchicalLifetimeManager());
            Container.RegisterType<IDiagnosticProvider, ErrorListDiagnosticProvider>(new ContainerControlledLifetimeManager());
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }
    }
}
