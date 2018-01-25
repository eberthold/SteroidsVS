namespace SteroidsVS.CodeStructure
{
    using System;
    using Microsoft.VisualStudio.Text.Editor;
    using Steroids.CodeStructure.Adorners;
    using Steroids.CodeStructure.Analyzers.Services;
    using Steroids.CodeStructure.ViewModels;
    using Unity;
    using Unity.Lifetime;

    public class CodeStructureCompositionRoot : CompositionRoot, IDisposable
    {
        private readonly IWpfTextView _textView;

        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureCompositionRoot"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to analyze.</param>
        public CodeStructureCompositionRoot(IWpfTextView textView)
        {
            _textView = textView;
            Initialize();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Container.Dispose();
                    Container = null;
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            Container = RootContainer.CreateChildContainer();
            Container.RegisterInstance(_textView);

            Container.RegisterType<ISyntaxWalkerProvider, SyntaxWalkerProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureAdorner>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<FloatingDiagnosticHintsAdorner>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeQualityHintsViewModel>(new ContainerControlledLifetimeManager());
        }
    }
}
