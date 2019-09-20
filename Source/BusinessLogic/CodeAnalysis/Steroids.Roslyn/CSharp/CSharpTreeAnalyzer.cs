using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Steroids.Roslyn.StructureAnalysis;

namespace Steroids.Roslyn.CSharp
{
    public class CSharpTreeAnalyzer : RoslynTreeAnalyzer<MemberDeclarationSyntax>
    {
        /// <inheritdoc />
        protected override IReadOnlyCollection<Type> KnownNodeTypes => CSharpNodeMapper.KnownNodeTypes;

        /// <inheritdoc />
        protected override IRoslynNodeMapper NodeMapper { get; } = new CSharpNodeMapper();

        /// <inheritdoc />
        public override SyntaxTree ParseText(string text)
        {
            return CSharpSyntaxTree.ParseText(text);
        }

        /// <inheritdoc />
        protected override bool NeedsMetaNode(MemberDeclarationSyntax member)
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
