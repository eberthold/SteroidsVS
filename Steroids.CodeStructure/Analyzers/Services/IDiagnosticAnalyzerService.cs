namespace Steroids.CodeStructure.Analyzers.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public interface IDiagnosticAnalyzerService
    {
        event EventHandler CompilationFinished;

        CompilationWithAnalyzers GetProjectCompilationWithAnalyzers(Project project);

        IEnumerable<Diagnostic> GetProjectDiagnostics(Project project);

        /// <summary>
        /// This triggers a new compilation run.
        /// All ongoing compilations will be canceled.
        /// Once the compilation is done, the <see cref="CompilationFinished"/> event is fired.
        /// </summary>
        void DoCompilation();
    }
}
