namespace Steroids.CodeStructure.Analyzers.SectionHeader
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Imaging.Interop;

    public class InterfaceSectionHeader : SectionHeaderBase
    {
        public override bool IsMetaNode => false;

        public override int SectionTypeOrderIndex => 3;

        protected override ImageMoniker GetMoniker()
        {
            return (ImageMoniker)typeof(KnownMonikers).GetProperty($"Interface{AccessModifier}").GetValue(null);
        }

        protected override string GetName()
        {
            var node = Node as InterfaceDeclarationSyntax;
            if (node == null)
            {
                return "Interface";
            }

            return node.Identifier.ValueText;
        }
    }
}