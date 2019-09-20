using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Steroids.Roslyn.StructureAnalysis;

namespace Steroids.Roslyn.VisualBasic
{
    public class VisualBasicTreeAnalyzer : RoslynTreeAnalyzer<DeclarationStatementSyntax>
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> KnownNodeTypes => VisualBasicNodeMapper.KnownNodeTypes;

        /// <inheritdoc />
        protected override IRoslynNodeMapper NodeMapper { get; } = new VisualBasicNodeMapper();

        /// <inheritdoc />
        public override SyntaxTree ParseText(string text)
        {
            return VisualBasicSyntaxTree.ParseText(text);
        }

        /// <inheritdoc />
        protected override bool NeedsMetaNode(DeclarationStatementSyntax member)
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
