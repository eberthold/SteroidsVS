using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.Contracts;

namespace Steroids.Core.Extensions
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
        public static ITextSnapshotLine GetSpanForLineNumber(this IWpfTextView textView, int lineNumber)
        {
            if (textView.TextSnapshot.LineCount <= lineNumber)
            {
                return null;
            }

            return textView.TextSnapshot.GetLineFromLineNumber(lineNumber);
        }

        public static IEnumerable<DiagnosticInfo> ExtractRelatedDiagnostics(this IWpfTextView textView, IEnumerable<DiagnosticInfo> diagnostics)
        {
            var path = textView.GetDocument()?.FilePath;
            if (string.IsNullOrWhiteSpace(path))
            {
                return Enumerable.Empty<DiagnosticInfo>();
            }

            return diagnostics.Where(x => string.Equals(x.Path, path, StringComparison.OrdinalIgnoreCase));
        }
    }
}
