namespace Steroids.CodeStructure.Analyzers.NodeContainer
{
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Imaging.Interop;
    using Steroids.CodeStructure.Extensions;

    public class MethodNodeContainer : NodeContainerBase<MethodDeclarationSyntax>, ICodeStructureLeaf
    {
        /// <inheritdoc />
        protected override string GetAccessModifier()
        {
            return Node.Modifiers.GetAccessModifier();
        }

        /// <inheritdoc />
        protected override ImageMoniker GetMoniker()
        {
            return (ImageMoniker)typeof(KnownMonikers).GetProperty($"Method{AccessModifier}").GetValue(null);
        }

        /// <inheritdoc />
        protected override string GetName()
        {
            return Node.Identifier.ValueText;
        }

        /// <summary>
        /// Gets the <see cref="ICodeStructureNodeContainer.Parameters"/>
        /// </summary>
        protected override string GetParameters()
        {
            return string.Join(string.Empty, Node.ParameterList.Parameters.Select(x => x.Type.ToFullString()));
        }

        /// <inheritdoc />
        protected override string GetReturnType()
        {
            return Node.ReturnType.ToFullString();
        }
    }
}