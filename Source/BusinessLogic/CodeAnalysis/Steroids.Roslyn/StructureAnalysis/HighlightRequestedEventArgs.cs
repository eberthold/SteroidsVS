using System;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.StructureAnalysis
{
    public class HighlightRequestedEventArgs : EventArgs
    {
        public HighlightRequestedEventArgs(CodeStructureItem nodeContainer)
        {
            NodeContainer = nodeContainer;
        }

        /// <summary>
        /// The node to highlight.
        /// </summary>
        public CodeStructureItem NodeContainer { get; }
    }
}
