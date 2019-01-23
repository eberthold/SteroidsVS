using System;
using System.Collections.Generic;

namespace Steroids.Code.CodeQuality
{
    public class DiagnosticsChangedEventArgs : EventArgs
    {
        public DiagnosticsChangedEventArgs(IReadOnlyCollection<DiagnosticInfo> readOnlyCollection)
        {
            Diagnostics = readOnlyCollection;
        }

        public IReadOnlyCollection<DiagnosticInfo> Diagnostics { get; set; }
    }
}
