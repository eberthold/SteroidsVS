﻿namespace Steroids.CodeStructure.Analyzers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

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
        /// Gets the diagnostics of the last analysis run.
        /// </summary>
        IEnumerable<Diagnostic> Diagnostics { get; }
    }
}