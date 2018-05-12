using Microsoft.VisualStudio.Text.Editor;

namespace Steroids.Contracts.Core
{
    public interface IActiveTextViewProvider
    {
        /// <summary>
        /// Gets the active text view or null.
        /// </summary>
        IWpfTextView ActiveTextView { get; }
    }
}
