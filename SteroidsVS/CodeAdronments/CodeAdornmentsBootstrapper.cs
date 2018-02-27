using Steroids.Contracts.UI;
using System;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.CodeQuality.Adorners;
using Steroids.CodeQuality.Models;
using Steroids.CodeQuality.ViewModels;
using Steroids.CodeStructure.Adorners;
using Steroids.CodeStructure.Analyzers.Services;
using Steroids.CodeStructure.UI;
using Unity;
using Unity.Lifetime;

namespace SteroidsVS.CodeAdornments
{
    public class CodeAdornmentsBootstrapper : Bootstrapper, IDisposable
    {
        private IWpfTextView _textView;

        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeAdornmentsBootstrapper"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to analyze.</param>
        public CodeAdornmentsBootstrapper(IWpfTextView textView)
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
                    _textView = null;
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
            Container.RegisterInstance(_textView.GetAdornmentLayer(nameof(CodeStructureAdorner)));

            Container.RegisterType<IAdornmentSpaceReservation, CodeStructureSpaceReservation>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDocumentAnalyzerService, DocumentAnalyzerService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISyntaxWalkerProvider, SyntaxWalkerProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureAdorner>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeQualityHintsViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureViewFactory>(new ContainerControlledLifetimeManager());

            Container.RegisterType<FloatingDiagnosticHintsAdorner>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeHintFactory>(new ContainerControlledLifetimeManager());
        }
    }
}
