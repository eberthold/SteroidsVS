namespace Steroids.CodeStructure.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class DiagnosticsChangedEventArgs : EventArgs
    {
        public DiagnosticsChangedEventArgs(ReadOnlyCollection<DiagnosticInfo> readOnlyCollection)
        {
            Diagnostics = readOnlyCollection;
        }

        public IReadOnlyList<DiagnosticInfo> Diagnostics { get; set; }
    }
}
