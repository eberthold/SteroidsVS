namespace Steroids.CodeStructure.Analyzers.NodeContainer
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Imaging.Interop;
    using Steroids.CodeStructure.Extensions;

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
            return (ImageMoniker)typeof(KnownMonikers).GetProperty($"Property{AccessModifier}").GetValue(null);
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