namespace SteroidsVS
{
    using System;
    using System.ComponentModel.Composition;
    using EnvDTE;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Shell;
    using Steroids.CodeStructure.Analyzers.Services;
    using Steroids.Contracts;
    using SteroidsVS.Contracts;
    using Unity;
    using Unity.Injection;
    using Unity.Lifetime;

    [Export(typeof(ICompositionRoot))]
    public class CompositionRoot : IServiceProvider, ICompositionRoot
    {
        protected static IUnityContainer RootContainer { get; private set; }

        protected virtual IUnityContainer Container { get; set; }

        public void Initialize(VisualStudioWorkspace workspace)
        {
            RootContainer = new UnityContainer();
            Container = RootContainer;

            Container.RegisterInstance(workspace);

            Container.RegisterType<IWorkspaceManager, WorkspaceManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDocumentAnalyzerService, DocumentAnalyzerService>(new HierarchicalLifetimeManager());
            Container.RegisterType<ICompilationAnalyzerService, CompilationAnalyzerService>(new ContainerControlledLifetimeManager());
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }
    }
}
