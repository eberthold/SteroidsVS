using System;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.CodeStructure.UI
{
    public class HighlightRequestedEventArgs : EventArgs
    {
        public HighlightRequestedEventArgs(ICodeStructureNodeContainer nodeContainer)
        {
            NodeContainer = nodeContainer;
        }

        /// <summary>
        /// The node to highlight.
        /// </summary>
        public ICodeStructureNodeContainer NodeContainer { get; }
    }
}
