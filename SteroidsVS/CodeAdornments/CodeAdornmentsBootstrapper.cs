using System;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Steroids.CodeQuality.Adorners;
using Steroids.CodeQuality.Models;
using Steroids.CodeQuality.ViewModels;
using Steroids.CodeStructure.Adorners;
using Steroids.CodeStructure.Analyzers.Services;
using Steroids.CodeStructure.UI;
using Steroids.Contracts.UI;
using Steroids.Core.Diagnostics.Contracts;
using SteroidsVS.Models;
using Unity;
using Unity.Lifetime;

namespace SteroidsVS.CodeAdornments
{
    public class CodeAdornmentsBootstrapper : Bootstrapper, IDisposable
    {
        private IWpfTextView _textView;

        private bool _disposed;

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
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Container.Dispose();
                Container = null;
                _textView = null;
            }

            _disposed = true;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            Container = RootContainer.CreateChildContainer();

            var textViewWrapper = new TextViewWrapper(_textView);
            Container.RegisterInstance<IQualityTextView>(textViewWrapper);
            Container.RegisterInstance(_textView);
            Container.RegisterInstance(_textView.GetAdornmentLayer(nameof(CodeStructureAdorner)));

            var outliningManagerService = RootContainer.Resolve<IOutliningManagerService>();
            Container.RegisterInstance(outliningManagerService);

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
