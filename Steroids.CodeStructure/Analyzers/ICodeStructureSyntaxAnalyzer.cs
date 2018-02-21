using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Steroids.CodeStructure.Analyzers
{
    public interface ICodeStructureSyntaxAnalyzer
    {
        List<ICodeStructureNodeContainer> NodeList { get; }

        Task Analyze(SyntaxNode node, CancellationToken token);
    }
}