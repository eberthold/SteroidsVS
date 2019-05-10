using System;
using System.Collections.Generic;

namespace Steroids.Core.CodeQuality
{
    public interface IDiagnosticProvider
    {
        /// <summary>
        /// Is raised, when we received new diagnostics from the environment.
        /// </summary>
        event EventHandler<DiagnosticsChangedEventArgs> DiagnosticsChanged;

        /// <summary>
        /// Gets the last created set of diagnostics.
        /// </summary>
        IReadOnlyCollection<DiagnosticInfo> CurrentDiagnostics { get; }
    }
}
