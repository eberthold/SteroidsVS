using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Steroids.CodeStructure.Analyzers.SectionHeader
{
    public class StructSectionHeader : SectionHeaderBase
    {
        public override bool IsMetaNode => false;

        public override int SectionTypeOrderIndex => 2;

        protected override ImageMoniker GetMoniker()
        {
            return (ImageMoniker)typeof(KnownMonikers).GetProperty($"Structure{AccessModifier}").GetValue(null);
        }

        protected override string GetName()
        {
            var node = Node as StructDeclarationSyntax;
            if (node == null)
            {
                return "Struct";
            }

            return node.Identifier.ValueText;
        }
    }
}