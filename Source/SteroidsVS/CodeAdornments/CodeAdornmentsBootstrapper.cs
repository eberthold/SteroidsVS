using System;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Steroids.CodeQuality.UI;
using Steroids.CodeStructure.Analyzers;
using Steroids.CodeStructure.UI;
using Steroids.Core.UI;
using Steroids.Roslyn.StructureAnalysis;
using SteroidsVS.CodeQuality.UI;
using SteroidsVS.CodeStructure.Adorners;
using SteroidsVS.CodeStructure.UI;
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

            Container.RegisterInstance(_textView);
            Container.RegisterInstance(_textView.GetAdornmentLayer(nameof(CodeStructureAdorner)));

            var outliningManagerService = RootContainer.Resolve<IOutliningManagerService>();
            Container.RegisterInstance(outliningManagerService);

            Container.RegisterType<IAdornmentSpaceReservation, CodeStructureSpaceReservation>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDocumentAnalyzerService, DocumentAnalyzerService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureAdorner>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<DiagnosticInfosViewModel>(new ContainerControlledLifetimeManager());

            Container.RegisterType<DiagnosticInfoAdorner>(new ContainerControlledLifetimeManager());
        }
    }
}
