using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Steroids.CodeStructure.Analyzers;
using Steroids.Roslyn.Common;
using Steroids.Roslyn.Extensions;
using Steroids.Roslyn.StructureAnalysis;

namespace Steroids.Roslyn.CSharp
{
    internal class CSharpNodeMapper : IRoslynNodeMapper
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
            typeof(MethodDeclarationSyntax),
            typeof(EnumDeclarationSyntax),
            typeof(EnumMemberDeclarationSyntax)
        };

        public IReadOnlyCollection<CodeStructureItem> MapItem(SyntaxNode node)
        {
            switch (node)
            {
                case InterfaceDeclarationSyntax castNode:
                    return new[] { MapItem(castNode) };

                case StructDeclarationSyntax castNode:
                    return new[] { MapItem(castNode) };

                case ClassDeclarationSyntax castNode:
                    return new[] { MapItem(castNode) };

                case FieldDeclarationSyntax castNode:
                    return MapItem(castNode).ToList();

                case ConstructorDeclarationSyntax castNode:
                    return new[] { MapItem(castNode) };

                case EventFieldDeclarationSyntax castNode:
                    return MapItem(castNode).ToList();

                case PropertyDeclarationSyntax castNode:
                    return new[] { MapItem(castNode) };

                case MethodDeclarationSyntax castNode:
                    return new[] { MapItem(castNode) };

                case EnumDeclarationSyntax castNode:
                    return new[] { MapItem(castNode) };

                case EnumMemberDeclarationSyntax castNode:
                    return new[] { MapItem(castNode) };

                default:
                    return new[] 
                    {
                        new CodeStructureItem
                        {
                            Name = node.ToString()
                        }
                    };
            }
        }

        internal static CodeStructureItem MapItem(InterfaceDeclarationSyntax node)
        {
            var item = CreateItem<InterfaceNode>(node);
            item.AccessModifier = node.Modifiers.GetAccessModifier();
            item.Name = node.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(StructDeclarationSyntax node)
        {
            var item = CreateItem<StructNode>(node);
            item.AccessModifier = node.Modifiers.GetAccessModifier();
            item.Name = node.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(ClassDeclarationSyntax node)
        {
            var item = CreateItem<ClassNode>(node);
            item.AccessModifier = node.Modifiers.GetAccessModifier();
            item.Name = node.Identifier.Text;

            return item;
        }

        internal static IEnumerable<CodeStructureItem> MapItem(FieldDeclarationSyntax node)
        {
            foreach (var field in node.Declaration.Variables)
            {
                var item = CreateItem<FieldNode>(node);
                item.AccessModifier = node.Modifiers.GetAccessModifier();
                item.Name = field.Identifier.Text;

                yield return item;
            }
        }

        internal static CodeStructureItem MapItem(ConstructorDeclarationSyntax node)
        {
            var item = CreateItem<ConstructorNode>(node);
            item.AccessModifier = node.Modifiers.GetAccessModifier();
            item.Name = node.Identifier.Text;

            return item;
        }

        internal static IEnumerable<CodeStructureItem> MapItem(EventFieldDeclarationSyntax node)
        {
            foreach (var eventItem in node.Declaration.Variables)
            {
                var item = CreateItem<EventNode>(node);
                item.AccessModifier = node.Modifiers.GetAccessModifier();
                item.Name = eventItem.Identifier.Text;

                yield return item;
            }
        }

        internal static CodeStructureItem MapItem(PropertyDeclarationSyntax node)
        {
            var item = CreateItem<PropertyNode>(node);
            item.AccessModifier = node.Modifiers.GetAccessModifier();
            item.Name = node.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(MethodDeclarationSyntax node)
        {
            var item = CreateItem<MethodNode>(node);
            item.AccessModifier = node.Modifiers.GetAccessModifier();
            item.Name = node.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(EnumDeclarationSyntax node)
        {
            var item = CreateItem<EnumNode>(node);
            item.AccessModifier = node.Modifiers.GetAccessModifier();
            item.Name = node.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(EnumMemberDeclarationSyntax node)
        {
            var item = CreateItem<EnumMemberNode>(node);
            item.AccessModifier = "Public";
            item.Name = node.Identifier.Text;

            return item;
        }

        private static T CreateItem<T>(SyntaxNode syntaxNode)
            where T : CodeStructureItem, new()
        {
            var item = new T
            {
                StartLineNumber = GetStartLine(syntaxNode),
                EndLineNumber = GetEndLine(syntaxNode)
            };

            return item;
        }

        private static int GetStartLine(SyntaxNode syntaxNode)
        {
            return syntaxNode.GetLocation().GetLineSpan().StartLinePosition.Line;
        }

        private static int GetEndLine(SyntaxNode syntaxNode)
        {
            return syntaxNode.GetLocation().GetLineSpan().EndLinePosition.Line;
        }
    }
}
 