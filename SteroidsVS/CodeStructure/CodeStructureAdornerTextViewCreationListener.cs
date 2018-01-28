using System;
using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Steroids.CodeStructure.Adorners;
using Steroids.CodeStructure.ViewModels;

namespace SteroidsVS.CodeStructure
{
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    internal sealed class CodeStructureAdornerTextViewCreationListener : IWpfTextViewCreationListener
    {
        private IWpfTextView _textView;
        private CodeStructureBootstrapper _bootstrapper;

        /// <summary>
        /// Defines the adornment layer for the scarlet adornment. This layer is ordered
        /// after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("CodeStructureAdorner")]
        [Order(After = PredefinedAdornmentLayers.Caret)]
        private readonly AdornmentLayerDefinition _editorAdornmentLayer;

        /// <summary>
        /// Instantiates a CodeStructureAdorner manager when a textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            _textView = textView;
            _bootstrapper = new CodeStructureBootstrapper(textView);
            var viewModel = _bootstrapper.GetService(typeof(CodeStructureViewModel));
            var adorner = _bootstrapper.GetService(typeof(CodeStructureAdorner)) as CodeStructureAdorner;
            adorner.SetDataContext(viewModel);

            var adorner2 = _bootstrapper.GetService(typeof(FloatingDiagnosticHintsAdorner)) as FloatingDiagnosticHintsAdorner;

            WeakEventManager<ITextView, EventArgs>.AddHandler(textView, nameof(ITextView.Closed), OnClosed);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _textView?.GetAdornmentLayer(nameof(CodeStructureAdorner))?.RemoveAllAdornments();
            _textView = null;
            _bootstrapper?.Dispose();
            _bootstrapper = null;
        }
    }
}
