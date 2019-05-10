using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Steroids.CodeStructure.Analyzers
{
    public interface ICodeStructureSyntaxAnalyzer
    {
        /// <summary>
        /// Gets all nodes in the current file.
        /// </summary>
        IEnumerable<SortedTree<CodeStructureItem>> NodeList { get; }

        /// <summary>
        /// Analyze the current file.
        /// </summary>
        /// <param name="node">The node to analyze.</param>
        /// <param name="token">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Analyze(SyntaxNode node, CancellationToken token);
    }
}