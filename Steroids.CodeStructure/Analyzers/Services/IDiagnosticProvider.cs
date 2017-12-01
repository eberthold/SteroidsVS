namespace Steroids.CodeStructure.Analyzers.Services
{
    using System;

    public interface IDiagnosticProvider
    {
        event EventHandler<DiagnosticsChangedEventArgs> DiagnosticsChanged;
    }
}
