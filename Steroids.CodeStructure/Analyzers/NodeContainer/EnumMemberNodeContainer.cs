namespace Steroids.CodeStructure.Analyzers.NodeContainer
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Imaging.Interop;

    public class EnumMemberNodeContainer : NodeContainerBase<EnumMemberDeclarationSyntax>, ICodeStructureLeaf
    {
        /// <inheritdoc />
        protected override string GetAccessModifier()
        {
            return "Public";
        }

        /// <inheritdoc />
        protected override ImageMoniker GetMoniker()
        {
            return MonikerCache.GetMoniker($"EnumerationItem{AccessModifier}");
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
            return string.Empty;
        }
    }
}