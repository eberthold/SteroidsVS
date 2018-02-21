using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Imaging.Interop;
using Steroids.CodeStructure.Extensions;

namespace Steroids.CodeStructure.Analyzers.NodeContainer
{
    public class PropertyNodeContainer : NodeContainerBase<PropertyDeclarationSyntax>, ICodeStructureLeaf
    {
        /// <inheritdoc />
        protected override string GetAccessModifier()
        {
            return Node.Modifiers.GetAccessModifier();
        }

        /// <inheritdoc />
        protected override ImageMoniker GetMoniker()
        {
            return MonikerCache.GetMoniker($"Property{AccessModifier}");
        }

        /// <inheritdoc />
        protected override string GetName()
        {
            return Node.Identifier.ValueText;
        }

        protected override string GetParameters()
        {
            return string.Empty;
        }

        /// <inheritdoc />
        protected override string GetReturnType()
        {
            return Node.Type.ToFullString();
        }
    }
}