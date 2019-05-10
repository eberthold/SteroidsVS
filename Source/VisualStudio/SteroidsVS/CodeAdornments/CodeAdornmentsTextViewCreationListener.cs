using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using SteroidsVS.CodeQuality.UI;
using SteroidsVS.CodeStructure.Adorners;

namespace SteroidsVS.CodeAdornments
{
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation.
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    internal sealed class CodeAdornmentsTextViewCreationListener : IWpfTextViewCreationListener
    {
#pragma warning disable RCS1213 // Remove unused member declaration.
#pragma warning disable CS0169 // The field 'CodeAdornmentsTextViewCreationListener._editorAdornmentLayer' is never used
        [Export(typeof(AdornmentLayerDefinition))]
        [Name(nameof(CodeStructureAdorner))]
        [Order(After = PredefinedAdornmentLayers.Caret)]
        private readonly AdornmentLayerDefinition _editorAdornmentLayer;
#pragma warning restore CS0169 // The field 'CodeAdornmentsTextViewCreationListener._editorAdornmentLayer' is never used
#pragma warning restore RCS1213 // Remove unused member declaration.

        private readonly Dictionary<IWpfTextView, CodeAdornmentsBootstrapper> _cleanupMap = new Dictionary<IWpfTextView, CodeAdornmentsBootstrapper>();

        /// <summary>
        /// Instantiates a CodeStructureAdorner manager when a textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed.</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            SteroidsVsPackage.EnsurePackageLoadedAsync().ContinueWith(
                t =>
                {
                    var bootstrapper = new CodeAdornmentsBootstrapper(textView);
                    if (_cleanupMap.ContainsKey(textView))
                    {
                        return;
                    }

                    _cleanupMap.Add(textView, bootstrapper);

                    var codeStructure = bootstrapper.GetService(typeof(CodeStructureAdorner)) as CodeStructureAdorner;
                    var diagnosticHints = bootstrapper.GetService(typeof(DiagnosticInfoAdorner)) as DiagnosticInfoAdorner;
                    WeakEventManager<ITextView, EventArgs>.AddHandler(textView, nameof(ITextView.Closed), OnClosed);
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Cleaning up our resources.
        /// </summary>
        /// <param name="sender">The sender, which should always be an <see cref="IWpfTextView"/>.</param>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        private void OnClosed(object sender, EventArgs e)
        {
            var textView = sender as IWpfTextView;
            if (textView == null)
            {
                return;
            }

            textView.GetAdornmentLayer(nameof(CodeStructureAdorner))?.RemoveAllAdornments();
            if (!_cleanupMap.ContainsKey(textView))
            {
                return;
            }

            var bootstrapper = _cleanupMap[textView];
            bootstrapper?.Dispose();

            _cleanupMap.Remove(textView);
        }
    }
}
