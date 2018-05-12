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
        /// Gets the Id of the Project, for which this Analyzer works.
        /// </summary>
        ProjectId ProjectId { get; }
    }
}
