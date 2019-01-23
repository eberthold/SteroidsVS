using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace SteroidsVs.UI.Editor
{
    public static class IWpfTextViewExtensions
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

        /// <summary>
        /// Gets the <see cref="ITextSnapshotLine"/> for the given line number.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>The corresponding <see cref="ITextSnapshotLine"/> or null.</returns>
        public static ITextSnapshotLine GetSnapshotForLineNumber(this IWpfTextView textView, int lineNumber)
        {
            if (textView.TextSnapshot.LineCount <= lineNumber)
            {
                return null;
            }

            try
            {
                return textView.TextSnapshot.GetLineFromLineNumber(lineNumber);
            }
            catch
            {
                // the GetLineFromLineNumber throws diverse exceptions which hint at wrong index, even with guard clause
                // I'm not happy with this general catch, but it's better than error messages.
            }

            return null;
        }

        /// <summary>
        /// Extracts the <see cref="DiagnosticInfo"/> Elements which are related to this document.
        /// </summary>
        /// <param name="editor">The <see cref="IWpfTextView"/>.</param>
        /// <param name="diagnostics">The list of all <see cref="DiagnosticInfo"/>.</param>
        /// <returns>All related <see cref="DiagnosticInfo"/>.</returns>
        //public static IEnumerable<DiagnosticInfo> ExtractRelatedDiagnostics(this IEditorImplementation editor, IEnumerable<DiagnosticInfo> diagnostics)
        //{
        //    var path = editor.FilePath;
        //    if (string.IsNullOrWhiteSpace(path))
        //    {
        //        return Enumerable.Empty<DiagnosticInfo>();
        //    }

        //    return diagnostics.Where(x => path.EndsWith(x?.Path ?? " ", StringComparison.OrdinalIgnoreCase));
        //}
    }
}
