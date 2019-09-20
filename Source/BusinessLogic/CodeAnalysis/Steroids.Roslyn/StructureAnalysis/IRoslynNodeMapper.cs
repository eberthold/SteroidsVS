using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.StructureAnalysis
{
    public interface IRoslynNodeMapper
    {
        /// <summary>
        /// Maps a Roslyn <see cref="SyntaxNode"/> to Steroids <see cref="CodeStructureItem"/>.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/> to map.</param>
        /// <returns>The list of resulting <see cref="CodeStructureItem"/>.</returns>
        IReadOnlyCollection<CodeStructureItem> MapItem(SyntaxNode node);
    }
}