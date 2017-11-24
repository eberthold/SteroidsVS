namespace Steroids.CodeStructure.Analyzers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Empty sub-interface to distinguish headers and nodes
    /// </summary>
    public interface ICodeStructureSectionHeader : ICodeStructureNodeContainer
    {
        /// <summary>
        /// Gets or sets a value indicating whether this node should be collapsed.
        /// </summary>
        bool IsCollapsed { get; set; }

        /// <summary>
        /// Gets or sets the id of the syntax walker which manages this node.
        /// Used as internal field for managing the code structure list.
        /// </summary>
        Guid SyntaxWalkerId { get; set; }

        /// <summary>
        /// Gets the order in which section types should be appear.
        /// e.g. fields before constructors etc.
        /// </summary>
        int SectionTypeOrderIndex { get; }

        /// <summary>
        /// Gets the children of this section
        /// </summary>
        List<ICodeStructureNodeContainer> Items { get; }

        /// <summary>
        /// Gets the children of this section
        /// </summary>
        List<ICodeStructureNodeContainer> AllTreeItems { get; }

        /// <summary>
        /// Gets a value indicating whether this node is only a meta node to group similar items.
        /// </summary>
        bool IsMetaNode { get; }

        /// <summary>
        /// Refreshes the indexes information on all child elements.
        /// </summary>
        void RefreshIndexes();
    }
}
