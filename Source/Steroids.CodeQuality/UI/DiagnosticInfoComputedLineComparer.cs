using System.Collections.Generic;
using Steroids.Contracts;

namespace Steroids.CodeQuality.UI
{
    /// <summary>
    /// Alternates the default <see cref="DiagnosticInfo"/> comparison by switching to <see cref="DiagnosticInfo.ComputedLineNumber"/> instead of <see cref="DiagnosticInfo.LineNumber"/>.
    /// </summary>
    internal class DiagnosticInfoComputedLineComparer : IComparer<DiagnosticInfo>
    {
        /// <inheritdoc />
        public int Compare(DiagnosticInfo x, DiagnosticInfo y)
        {
            var line = x.ComputedLineNumber - y.ComputedLineNumber;
            if (line != 0)
            {
                return line;
            }

            if (x.Severity > y.Severity)
            {
                return -1;
            }
            else if (x.Severity < y.Severity)
            {
                return 1;
            }

            return x.Column - y.Column;
        }
    }
}
