using System;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Steroids.CodeStructure.Analyzers
{
    public interface ICodeStructureNodeContainer
    {
        /// <summary>
        /// Gets the level of indentation.
        /// </summary>
        int IndentLevel { get; }

        /// <summary>
        /// Gets the access modifier of this node (e.g. public, private).
        /// </summary>
        string AccessModifier { get; }

        /// <summary>
        /// Gets the parameters of this node, if any.
        /// </summary>
        string Parameters { get; }

        /// <summary>
        /// Gets the last index which is used in this section.
        /// </summary>
        int LastAbsoluteIndex { get; }

        /// <summary>
        /// Gets the name of this node.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of this node. Could be either property type or method return type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets or sets the <see cref="SyntaxNode"/>.
        /// </summary>
        SyntaxNode Node { get; set; }

        /// <summary>
        /// Gets or sets the unique analyze run id.
        /// Is Only used as internal field, to flag nodes which may could be deleted.
        /// </summary>
        Guid AnalyzeId { get; set; }

        /// <summary>
        /// Gets the moniker of this node.
        /// </summary>
        ImageMoniker Moniker { get; }

        /// <summary>
        /// Gets or sets the parent of this node.
        /// </summary>
        ICodeStructureNodeContainer Parent { get; set; }

        /// <summary>
        /// Gets a string, that should be unique to identify a single unique object.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets or sets the absolute index of this container in a flat list.
        /// </summary>
        int AbsoluteIndex { get; set; }

        /// <summary>
        /// The beginning of the node in file.
        /// </summary>
        int StartLineNumber { get; set; }

        /// <summary>
        /// The end of the node in file.
        /// </summary>
        int EndLineNumber { get; set; }
    }
}
