using System;
using System.Collections.Generic;

namespace Steroids.CodeStructure.Analyzers
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
        IEnumerable<SortedTree<ICodeStructureItem>> Nodes { get; }

        /// <summary>
        /// Tells, if we are able to analyze this kind of document right now.
        /// </summary>
        bool IsAnalyzeable { get; }
    }
}
