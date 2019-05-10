using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.StructureAnalysis
{
    public class CSharpTreeAnalyzer : ICodeStructureSyntaxAnalyzer
    {
        /// <inheritdoc />
        public IEnumerable<SortedTree<CodeStructureItem>> NodeList { get; private set; }

        /// <inheritdoc />
        public Task Analyze(SyntaxNode node, CancellationToken token)
        {
            var root = new SortedTree<CodeStructureItem>(new CodeStructureItem() { Name = "File" });
            var memberDeclarations = node
                .DescendantNodes(_ => true)
                .OfType<MemberDeclarationSyntax>()
                .Where(x => CSharpNodeMapper.KnownNodeTypes.Contains(x.GetType()));
            
            foreach (var declaration in memberDeclarations)
            {
                foreach (var mappedItem in CSharpNodeMapper.MapItem(declaration))
                {
                    var element = new SortedTree<CodeStructureItem>(mappedItem, declaration);
                    var parent = root.FirstOrDefault(x => x.Meta == declaration.Parent) ?? root;
                    if (NeedsMetaNode(declaration))
                    {
                        var realParent = parent.Children.FirstOrDefault(x => x.Data.GetType() == element.Data.GetType() && x.Data.IsMeta);
                        if (realParent is null)
                        {
                            var metaData = CSharpMetaNodeCreator.Create(element.Data);
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

        private bool NeedsMetaNode(MemberDeclarationSyntax member)
        {
            switch (member)
            {
                case FieldDeclarationSyntax _:
                case ConstructorDeclarationSyntax _:
                case EventFieldDeclarationSyntax _:
                case PropertyDeclarationSyntax _:
                case MethodDeclarationSyntax _:
                    return true;

                default:
                    return false;
            }
        }
    }
}
