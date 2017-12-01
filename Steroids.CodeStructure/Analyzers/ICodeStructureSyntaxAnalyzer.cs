namespace Steroids.CodeStructure.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    public interface ICodeStructureSyntaxAnalyzer
    {
        List<ICodeStructureNodeContainer> NodeList { get; }

        Task Analyze(SyntaxNode node, CancellationToken token);
    }
}