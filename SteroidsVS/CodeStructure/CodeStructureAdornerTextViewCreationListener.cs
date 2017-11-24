namespace SteroidsVS.CodeStructure
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;
    using Steroids.CodeStructure;
    using Steroids.CodeStructure.Adorners;

    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    internal sealed class CodeStructureAdornerTextViewCreationListener : IWpfTextViewCreationListener
    {
        /// <summary>
        /// Defines the adornment layer for the scarlet adornment. This layer is ordered
        /// after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("CodeStructureAdorner")]
        [Order(After = PredefinedAdornmentLayers.Caret)]
        private readonly AdornmentLayerDefinition EditorAdornmentLayer;

        /// <summary>
        /// Instantiates a CodeStructureAdorner manager when a textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            new CodeStructureCompositionRoot(textView).GetService(typeof(CodeStructureAdorner));
        }
    }
}
