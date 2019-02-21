using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Steroids.CodeStructure.Analyzers;
using Steroids.Roslyn.CSharp;

namespace Steroids.Roslyn.StructureAnalysis
{
    internal static class NodeMapper
    {
        internal static IReadOnlyCollection<Type> KnownNodeTypes { get; } = new List<Type>
        {
            typeof(InterfaceDeclarationSyntax),
            typeof(StructDeclarationSyntax),
            typeof(ClassDeclarationSyntax),
            typeof(FieldDeclarationSyntax),
            typeof(ConstructorDeclarationSyntax),
            typeof(EventFieldDeclarationSyntax),
            typeof(PropertyDeclarationSyntax),
            typeof(MethodDeclarationSyntax)
        };

        internal static IReadOnlyCollection<CodeStructureItem> MapItem(SyntaxNode node)
        {
            switch (node)
            {
                case InterfaceDeclarationSyntax castNode:
                    return new[] { MapItem(castNode)  };

                case StructDeclarationSyntax castNode:
                    return new [] { MapItem(castNode) };

                case ClassDeclarationSyntax castNode:
                    return new [] { MapItem(castNode) };

                case FieldDeclarationSyntax castNode:
                    return MapItem(castNode).ToList();

                case ConstructorDeclarationSyntax castNode:
                    return new [] { MapItem(castNode) };

                case EventFieldDeclarationSyntax castNode:
                    return MapItem(castNode).ToList();

                case PropertyDeclarationSyntax castNode:
                    return new [] { MapItem(castNode) };

                case MethodDeclarationSyntax castNode:
                    return new [] { MapItem(castNode) };

                default:
                    return new[] { new CodeStructureItem() };
            }
        }

        internal static CodeStructureItem MapItem(InterfaceDeclarationSyntax node)
        {
            return new CodeStructureItem
            {
                Name = node.Identifier.Text
            };
        }

        internal static CodeStructureItem MapItem(StructDeclarationSyntax node)
        {
            return new StructNode
            {
                Name = node.Identifier.Text
            };
        }

        internal static CodeStructureItem MapItem(ClassDeclarationSyntax node)
        {
            return new ClassNode
            {
                Name = node.Identifier.Text
            };
        }

        internal static IEnumerable<CodeStructureItem> MapItem(FieldDeclarationSyntax node)
        {
            foreach (var field in node.Declaration.Variables)
            {
                yield return new FieldNode
                {
                    Name = field.Identifier.Text
                };
            }
        }

        internal static CodeStructureItem MapItem(ConstructorDeclarationSyntax node)
        {
            return new ConstructorNode
            {
                Name = node.Identifier.Text
            };
        }

        internal static IEnumerable<CodeStructureItem> MapItem(EventFieldDeclarationSyntax node)
        {
            foreach (var eventItem in node.Declaration.Variables)
            {
                yield return new EventNode
                {
                    Name = eventItem.Identifier.Text
                };
            }
        }

        internal static CodeStructureItem MapItem(PropertyDeclarationSyntax node)
        {
            return new PropertyNode
            {
                Name = node.Identifier.Text
            };
        }

        internal static CodeStructureItem MapItem(MethodDeclarationSyntax node)
        {
            return new MethodNode
            {
                Name = node.Identifier.Text
            };
        }
    }
}
