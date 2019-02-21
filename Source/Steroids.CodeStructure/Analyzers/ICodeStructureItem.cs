using System;

namespace Steroids.CodeStructure.Analyzers
{
    public interface ICodeStructureItem : IComparable<ICodeStructureItem>
    {
        /// <summary>
        /// Gets the access modifier of this node (e.g. public, private).
        /// </summary>
        string AccessModifier { get; set; }

        /// <summary>
        /// Gets the parameters of this node, if any.
        /// </summary>
        string Parameters { get; set; }

        /// <summary>
        /// Gets the name of this node.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the type of this node. Could be either property type or method return type.
        /// </summary>
        string ReturnType { get; set; }

        /// <summary>
        /// Gets or sets the unique analyze run id.
        /// Is Only used as internal field, to flag nodes which may could be deleted.
        /// </summary>
        Guid AnalyzeId { get; set; }

        /// <summary>
        /// The type of node this container represents e.g. "Class" in C#.
        /// </summary>
        string NodeType { get; set; }

        /// <summary>
        /// The beginning of the node in file.
        /// </summary>
        int StartLineNumber { get; set; }

        /// <summary>
        /// The end of the node in file.
        /// </summary>
        int EndLineNumber { get; set; }

        /// <summary>
        /// <see langword="true"/> if this node doesn't represent a real code item.
        /// </summary>
        bool IsMeta { get; set; }

        /// <summary>
        /// Tells in which order this <see cref="NodeType"/> should be placed.
        /// </summary>
        int OrderBaseValue { get; }
    }
}
