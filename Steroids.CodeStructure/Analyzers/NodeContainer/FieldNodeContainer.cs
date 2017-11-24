namespace Steroids.CodeStructure.Analyzers.NodeContainer
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Imaging.Interop;
    using Steroids.CodeStructure.Extensions;

    public class FieldNodeContainer : NodeContainerBase<VariableDeclaratorSyntax>, ICodeStructureLeaf
    {
        /// <inheritdoc />
        protected override string GetAccessModifier()
        {
            var field = Node?.Parent?.Parent as FieldDeclarationSyntax;
            if (field == null)
            {
                return string.Empty;
            }

            return field.Modifiers.GetAccessModifier();
        }

        /// <inheritdoc />
        protected override ImageMoniker GetMoniker()
        {
            return MonikerCache.GetMoniker($"Field{AccessModifier}");
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
            var field = Node?.Parent?.Parent as FieldDeclarationSyntax;
            if (field == null)
            {
                return string.Empty;
            }

            return string.Empty;
        }
    }
}