using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.Contracts;

namespace Steroids.Core.Extensions
{
    public static class IWpfTextViewHelper
    {
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
