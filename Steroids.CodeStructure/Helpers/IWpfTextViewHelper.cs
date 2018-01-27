﻿namespace Steroids.CodeStructure.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Text.Editor;
    using Steroids.CodeStructure.Analyzers;
    using Steroids.CodeStructure.Extensions;

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
