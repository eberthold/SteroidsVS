using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Steroids.CodeStructure.Analyzers.SectionHeader
{
    public class ClassSectionHeader : SectionHeaderBase
    {
        public override bool IsMetaNode => false;

        public override int SectionTypeOrderIndex => 4;

        protected override ImageMoniker GetMoniker()
        {
            return (ImageMoniker)typeof(KnownMonikers).GetProperty($"Class{AccessModifier}").GetValue(null);
        }

        protected override string GetName()
        {
            var node = Node as ClassDeclarationSyntax;
            if (node == null)
            {
                return "Class";
            }

            return node.Identifier.ValueText;
        }
    }
}