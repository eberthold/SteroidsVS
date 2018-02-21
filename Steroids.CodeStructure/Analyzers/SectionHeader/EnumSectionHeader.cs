using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Steroids.CodeStructure.Analyzers.SectionHeader
{
    public class EnumSectionHeader : SectionHeaderBase
    {
        public override bool IsMetaNode => false;

        public override int SectionTypeOrderIndex => 1;

        protected override ImageMoniker GetMoniker()
        {
            return (ImageMoniker)typeof(KnownMonikers).GetProperty($"Enumeration{AccessModifier}").GetValue(null);
        }

        protected override string GetName()
        {
            var node = Node as EnumDeclarationSyntax;
            if (node == null)
            {
                return "Enum";
            }

            return node.Identifier.ValueText;
        }
    }
}