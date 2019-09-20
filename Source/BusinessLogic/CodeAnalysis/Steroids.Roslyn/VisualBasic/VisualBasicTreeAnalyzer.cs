using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Steroids.CodeStructure.Analyzers;
using Steroids.Roslyn.Common;
using Steroids.Roslyn.StructureAnalysis;

namespace Steroids.Roslyn.VisualBasic
{
    public class VisualBasicTreeAnalyzer : IRoslynTreeAnalyzer
    {
        /// <inheritdoc />
        public IEnumerable<SortedTree<CodeStructureItem>> NodeList { get; private set; }

        /// <inheritdoc />
        public Task Analyze(SyntaxNode node, CancellationToken token)
        {
            var root = new SortedTree<CodeStructureItem>(new CodeStructureItem() { Name = "File" });
            var memberDeclarations = node
                .DescendantNodes(_ => true)
                .OfType<DeclarationStatementSyntax>()
                .Where(x => VisualBasicNodeMapper.KnownNodeTypes.Contains(x.GetType()));

            foreach (var declaration in memberDeclarations)
            {
                foreach (var mappedItem in VisualBasicNodeMapper.MapItem(declaration))
                {
                    var element = new SortedTree<CodeStructureItem>(mappedItem, declaration);
                    var parent = root.FirstOrDefault(x => x.Meta == declaration.Parent) ?? root;
                    if (NeedsMetaNode(declaration))
                    {
                        var realParent = parent.Children.FirstOrDefault(x => x.Data.GetType() == element.Data.GetType() && x.Data.IsMeta);
                        if (realParent is null)
                        {
                            var metaData = RoslynMetaNodeCreator.Create(element.Data);
                            var metaNode = new SortedTree<CodeStructureItem>(metaData);
                            parent.Add(metaNode);
                            parent = metaNode;
                        }
                        else
                        {
                            parent = realParent;
                        }
                    }

                    parent.Add(element);
                }
            }

            NodeList = root.Skip(1).ToList();

            //var junctions = memberDeclarations.Select(x => x.Parent).Distinct();
            //var treeKnots = MapItems(junctions);
            //foreach (var knot in treeKnots)
            //{
            //    knot.Parent = treeKnots.FirstOrDefault(x => x.Data == )
            //}

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public SyntaxTree ParseText(string text)
        {
            return VisualBasicSyntaxTree.ParseText(text);
        }

        private bool NeedsMetaNode(DeclarationStatementSyntax member)
        {
            switch (member)
            {
                case FieldDeclarationSyntax _:
                case ConstructorBlockSyntax _:
                case EventBlockSyntax _:
                case PropertyStatementSyntax _:
                case MethodBlockSyntax _:
                    return true;

                default:
                    return false;
            }
        }
    }
}
