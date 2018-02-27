using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Steroids.Core.Extensions
{
    public static class WpfTextViewExtensions
    {
        /// <summary>
        /// Gets the <see cref="Document"/> of the <see cref="IWpfTextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        /// <returns>The corresponding <see cref="Document"/>.</returns>
        public static Document GetDocument(this IWpfTextView textView)
        {
            return textView.TextSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        }
    }
}
