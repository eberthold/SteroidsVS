namespace SteroidsVS.CodeStructure
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class SelectionHintAdornmentTextViewCreationListener : IWpfTextViewCreationListener
    {
        public const string AdornmentLayerName = "SelectionHintAdornment";

        [Export]
        [Name(AdornmentLayerName)]
        [Order(Before = PredefinedAdornmentLayers.Selection)]
        [TextViewRole(PredefinedTextViewRoles.Document)]
        public AdornmentLayerDefinition SelectionHintDefinition = null;

        public void TextViewCreated(IWpfTextView textView)
        {
        }
    }
}
