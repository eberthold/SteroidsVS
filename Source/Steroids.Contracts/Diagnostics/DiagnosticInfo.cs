using Microsoft.CodeAnalysis;

namespace Steroids.Contracts
{
    public class DiagnosticInfo
    {
        public DiagnosticSeverity Severity { get; set; }

        public string Message { get; set; }

        public string Path { get; set; }

        public string ErrorCode { get; set; }

        public string HelpUriRaw { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the DiagnosticInfo is not suppressed by comment or suppress attribute.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
