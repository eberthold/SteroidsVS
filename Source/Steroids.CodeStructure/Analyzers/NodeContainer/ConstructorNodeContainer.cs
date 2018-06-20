using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Imaging.Interop;
using Steroids.CodeStructure.Extensions;

namespace Steroids.CodeStructure.Analyzers.NodeContainer
{
    public class ConstructorNodeContainer : NodeContainerBase<ConstructorDeclarationSyntax>, ICodeStructureLeaf
    {
        /// <inheritdoc />
        protected override string GetAccessModifier()
        {
            return Node.Modifiers.GetAccessModifier();
        }

        /// <inheritdoc />
        protected override ImageMoniker GetMoniker()
        {
            return MonikerCache.GetMoniker($"Method{AccessModifier}");
        }

        /// <inheritdoc />
        protected override string GetName()
        {
            return Node.Identifier.ValueText;
        }

        /// <summary>
        /// Gets the <see cref="ICodeStructureNodeContainer.Parameters"/>.
        /// </summary>
        /// <returns>The types of the parameters given to this constructor.</returns>
        protected override string GetParameters()
        {
            return string.Join(string.Empty, Node.ParameterList.Parameters.Select(x => x.Type.ToFullString()));
        }

        /// <inheritdoc />
        protected override string GetReturnType()
        {
            return string.Empty;
        }
    }
}