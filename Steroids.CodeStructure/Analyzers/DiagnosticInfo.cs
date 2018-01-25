namespace Steroids.CodeStructure.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.Text;

    public class DiagnosticInfo
    {
        public DiagnosticSeverity Severity { get; internal set; }

        public string Message { get; internal set; }

        public string Path { get; internal set; }

        public string ErrorCode { get; internal set; }

        public string HelpUriRaw { get; internal set; }

        public int Line { get; internal set; }

        public int Column { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the DiagnosticInfo is not suppressed by comment or suppress attribute.
        /// </summary>
        public bool IsActive { get; internal set; }

        public ITextSnapshotLine LineSpan { get; internal set; }
    }
}
