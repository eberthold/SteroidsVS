using System;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.StructureAnalysis
{
    public class HighlightRequestedEventArgs : EventArgs
    {
        public HighlightRequestedEventArgs(ICodeStructureItem nodeContainer)
        {
            NodeContainer = nodeContainer;
        }

        /// <summary>
        /// The node to highlight.
        /// </summary>
        public ICodeStructureItem NodeContainer { get; }
    }
}
