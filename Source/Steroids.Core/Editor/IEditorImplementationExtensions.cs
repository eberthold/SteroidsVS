using System;
using System.Collections.Generic;
using System.Linq;
using Steroids.Core.CodeQuality;

namespace Steroids.Core.Editor
{
    /// <summary>
    /// Provide generally useable extension methods for <see cref="IEditorImplementation"/>.
    /// </summary>
    public static class IEditorImplementationExtensions
    {
        /// <summary>
        /// Extracts the <see cref="DiagnosticInfo"/> Elements which are related to this document.
        /// </summary>
        /// <param name="editor">The <see cref="IWpfTextView"/>.</param>
        /// <param name="diagnostics">The list of all <see cref="DiagnosticInfo"/>.</param>
        /// <returns>All related <see cref="DiagnosticInfo"/>.</returns>
        public static IEnumerable<DiagnosticInfo> ExtractRelatedDiagnostics(this IEditorImplementation editor, IEnumerable<DiagnosticInfo> diagnostics)
        {
            var path = editor.FilePath;
            if (string.IsNullOrWhiteSpace(path))
            {
                return Enumerable.Empty<DiagnosticInfo>();
            }

            return diagnostics.Where(x => path.EndsWith(x?.Path ?? " ", StringComparison.OrdinalIgnoreCase));
        }
    }
}
