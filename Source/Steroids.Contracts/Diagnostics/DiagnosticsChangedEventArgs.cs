using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Steroids.Contracts
{
    public class DiagnosticsChangedEventArgs : EventArgs
    {
        public DiagnosticsChangedEventArgs(ReadOnlyCollection<DiagnosticInfo> readOnlyCollection)
        {
            Diagnostics = readOnlyCollection;
        }

        public IReadOnlyList<DiagnosticInfo> Diagnostics { get; set; }
    }
}
