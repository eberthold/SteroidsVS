using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Steroids.CodeStructure.Analyzers;
using Steroids.Roslyn.Common;

namespace Steroids.Roslyn.StructureAnalysis
{
    public abstract class RoslynTreeAnalyzer<T> : IRoslynTreeAnalyzer
        where T : SyntaxNode
    {
        /// <inheritdoc />
        public IReadOnlyCollection<SortedTree<CodeStructureItem>> NodeList { get; private set; }

        /// <summary>
        /// Defines the list of known node types.
        /// </summary>
        protected abstract IReadOnlyCollection<Type> KnownNodeTypes { get; }

        /// <summary>
        /// The <see cref="IRoslynNodeMapper"/> to create <see cref="CodeStructureItem"/>s.
        /// </summary>
        protected abstract IRoslynNodeMapper NodeMapper { get; }

        /// <inheritdoc />
        public Task Analyze(SyntaxNode node, CancellationToken token)
        {
            if (node is null)
            {
                return Task.CompletedTask;
            }

            var root = new SortedTree<CodeStructureItem>(new CodeStructureItem() { Name = "File" });
            var memberDeclarations = node
                .DescendantNodes(_ => true)
                .OfType<T>()
                .Where(x => KnownNodeTypes.Contains(x.GetType()));

            foreach (var declaration in memberDeclarations)
            {
                foreach (var mappedItem in NodeMapper.MapItem(declaration))
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

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public abstract SyntaxTree ParseText(string text);

        /// <summary>
        /// Defines if the syntax node should be member of a met anode.
        /// </summary>
        /// <param name="node">The SyntaxNode.</param>
        /// <returns><see langword="true"/> if the node should be placed in a meta node.</returns>
        protected abstract bool NeedsMetaNode(T node);
    }
}
