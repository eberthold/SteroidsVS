using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Steroids.CodeStructure.Analyzers;
using Steroids.Roslyn.Common;
using Steroids.Roslyn.Extensions;

namespace Steroids.Roslyn.VisualBasic
{
    internal static class VisualBasicNodeMapper
    {
        internal static IReadOnlyCollection<Type> KnownNodeTypes { get; } = new List<Type>
        {
            typeof(InterfaceBlockSyntax),
            typeof(StructureBlockSyntax),
            typeof(ClassBlockSyntax),
            typeof(FieldDeclarationSyntax),
            typeof(ConstructorBlockSyntax),
            typeof(EventBlockSyntax),
            typeof(PropertyStatementSyntax),
            typeof(MethodBlockSyntax),
            typeof(EnumBlockSyntax),
            typeof(EnumMemberDeclarationSyntax),
            typeof(ModuleBlockSyntax)
        };

        internal static IReadOnlyCollection<CodeStructureItem> MapItem(SyntaxNode node)
        {
            switch (node)
            {
                case InterfaceBlockSyntax castNode:
                    return new[] { MapItem(castNode) };

                case StructureBlockSyntax castNode:
                    return new[] { MapItem(castNode) };

                case ClassBlockSyntax castNode:
                    return new[] { MapItem(castNode) };

                case FieldDeclarationSyntax castNode:
                    return MapItem(castNode).ToList();

                case ConstructorBlockSyntax castNode:
                    return new[] { MapItem(castNode) };

                case EventBlockSyntax castNode:
                    return new[] { MapItem(castNode) };

                case PropertyStatementSyntax castNode:
                    return new[] { MapItem(castNode) };

                case MethodBlockSyntax castNode:
                    return new[] { MapItem(castNode) };

                case EnumBlockSyntax castNode:
                    return new[] { MapItem(castNode) };

                case EnumMemberDeclarationSyntax castNode:
                    return new[] { MapItem(castNode) };

                case ModuleBlockSyntax castNode:
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

        internal static CodeStructureItem MapItem(InterfaceBlockSyntax node)
        {
            var item = CreateItem<InterfaceNode>(node);
            item.AccessModifier = node.InterfaceStatement.Modifiers.GetAccessModifier();
            item.Name = node.InterfaceStatement.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(StructureBlockSyntax node)
        {
            var item = CreateItem<StructNode>(node);
            item.AccessModifier = node.StructureStatement.Modifiers.GetAccessModifier();
            item.Name = node.StructureStatement.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(ClassBlockSyntax node)
        {
            var item = CreateItem<ClassNode>(node);
            item.AccessModifier = node.ClassStatement.Modifiers.GetAccessModifier();
            item.Name = node.ClassStatement.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(ModuleBlockSyntax node)
        {
            var item = CreateItem<ClassNode>(node);
            item.AccessModifier = node.ModuleStatement.Modifiers.GetAccessModifier();
            item.Name = node.ModuleStatement.Identifier.Text;

            return item;
        }

        internal static IEnumerable<CodeStructureItem> MapItem(FieldDeclarationSyntax node)
        {
            foreach (var field in node.Declarators.SelectMany(x => x.Names))
            {
                var item = CreateItem<FieldNode>(node);
                item.AccessModifier = node.Modifiers.GetAccessModifier();
                item.Name = field.Identifier.Text;

                yield return item;
            }
        }

        internal static CodeStructureItem MapItem(ConstructorBlockSyntax node)
        {
            var statement = node.BlockStatement as SubNewStatementSyntax;
            var item = CreateItem<ConstructorNode>(node);
            item.AccessModifier = statement.Modifiers.GetAccessModifier();
            item.Name = "New";

            return item;
        }

        internal static CodeStructureItem MapItem(EventBlockSyntax node)
        {
            var item = CreateItem<EventNode>(node);
            item.AccessModifier = node.EventStatement.Modifiers.GetAccessModifier();
            item.Name = node.EventStatement.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(PropertyStatementSyntax node)
        {
            var item = CreateItem<PropertyNode>(node);
            item.AccessModifier = node.Modifiers.GetAccessModifier();
            item.Name = node.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(MethodBlockSyntax node)
        {
            var item = CreateItem<MethodNode>(node);
            item.AccessModifier = node.SubOrFunctionStatement.Modifiers.GetAccessModifier();
            item.Name = node.SubOrFunctionStatement.Identifier.Text;

            return item;
        }

        internal static CodeStructureItem MapItem(EnumBlockSyntax node)
        {
            var item = CreateItem<EnumNode>(node);
            item.AccessModifier = node.EnumStatement.Modifiers.GetAccessModifier();
            item.Name = node.EnumStatement.Identifier.Text;

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