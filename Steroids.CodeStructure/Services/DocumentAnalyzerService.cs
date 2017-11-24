namespace Steroids.CodeStructure.Services
{
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Steroids.Contracts.Services;

    public class DocumentanAlyzerService : IDocumentAnalyzerService
    {
        /// <inheritdoc />
        public async Task<AnalysisResult> GetDiagnosticsAsync(Document document, CancellationToken token)
        {
            Compilation compilation = await document.Project.GetCompilationAsync();

            return await GetDiagnosticsWithAnalyzers(document, compilation, token);
        }

        private static Task<AnalysisResult> GetDiagnosticsWithAnalyzers(Document document, Compilation compilation, CancellationToken token)
        {
            var analyzerReferences = document.Project.AnalyzerReferences.SelectMany(x => x.GetAnalyzersForAllLanguages());
            var analyzers = ImmutableArray<DiagnosticAnalyzer>.Empty.AddRange(analyzerReferences);
            var analyzedCompilation = compilation.WithAnalyzers(analyzers);

            return analyzedCompilation.GetAnalysisResultAsync(token);
        }
    }
}
