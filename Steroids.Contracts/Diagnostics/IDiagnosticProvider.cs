using System;

namespace Steroids.Contracts
{
    public interface IDiagnosticProvider
    {
        event EventHandler<DiagnosticsChangedEventArgs> DiagnosticsChanged;
    }
}
