using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Steroids.CodeStructure.Analyzers.Services
{
    public interface IDocumentAnalyzerService
    {
        /// <summary>
        /// Raised when the analysis process is finished.
        /// </summary>
        event EventHandler AnalysisFinished;

        /// <summary>
        /// Gets all code structure nodes found in the last analysis run.
        /// </summary>
        IEnumerable<ICodeStructureNodeContainer> Nodes { get; }

        /// <summary>
        /// Tells, if we are able to analyze this kind of document right now.
        /// </summary>
        bool IsAnalyzeable { get; }
    }
}
