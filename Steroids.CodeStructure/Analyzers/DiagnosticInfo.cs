namespace Steroids.CodeStructure.Analyzers
{
    using Microsoft.VisualStudio.Shell.Interop;

    public class DiagnosticInfo
    {
        public __VSERRORCATEGORY ErrorCategory { get; internal set; }

        public string Message { get; internal set; }

        public string Path { get; internal set; }

        public string ErrorCode { get; internal set; }

        public string HelpUriRaw { get; internal set; }

        public int Line { get; internal set; }

        public int Column { get; internal set; }
    }
}
