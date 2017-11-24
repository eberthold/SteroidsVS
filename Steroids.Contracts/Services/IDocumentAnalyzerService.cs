namespace Steroids.Contracts.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public interface IDocumentAnalyzerService
    {
        /// <summary>
        /// Gets the <see cref="AnalysisResult"/> for the <see cref="Document"/>.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to analyze.</param>
        /// <param name="token">The <see cref="CancellationToken"/> to cancel this operation.</param>
        /// <returns>The <see cref="AnalysisResult"/>.</returns>
        Task<AnalysisResult> GetDiagnosticsAsync(Document document, CancellationToken token);
    }
}
