namespace SteroidsVS
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.CodeAnalysis;
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

        public void Initialize(Workspace workspace)
        {
            RootContainer = new UnityContainer();
            Container = RootContainer;

            Container.RegisterInstance(workspace);

            Container.RegisterType<IWorkspaceManager, WorkspaceManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDocumentAnalyzerService, DocumentAnalyzerService>(new HierarchicalLifetimeManager());
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }
    }
}
