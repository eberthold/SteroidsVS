using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace SteroidsVS.Services
{
    public class ActiveTextViewProvider : IActiveTextViewProvider
    {
        private readonly IVsTextManager _textManager;
        private readonly IVsEditorAdaptersFactoryService _editorAdapterFactory;

        public ActiveTextViewProvider(
            IVsTextManager textManager,
            IVsEditorAdaptersFactoryService editorAdapterFactory)
        {
            _textManager = textManager ?? throw new System.ArgumentNullException(nameof(textManager));
            _editorAdapterFactory = editorAdapterFactory ?? throw new System.ArgumentNullException(nameof(editorAdapterFactory));
        }

        /// <inheritdoc />
        public IWpfTextView ActiveTextView
        {
            get
            {
                _textManager.GetActiveView(1, null, out IVsTextView vsTextView);
                if (vsTextView == null)
                {
                    return null;
                }

                return _editorAdapterFactory.GetWpfTextView(vsTextView);
            }
        }
    }
}
