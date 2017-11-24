namespace Steroids.CodeStructure.Analyzers.SectionHeader
{
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Imaging.Interop;

    public class MethodsSectionHeader : SectionHeaderBase
    {
        public override bool IsMetaNode => true;

        public override int SectionTypeOrderIndex => 9;

        protected override ImageMoniker GetMoniker()
        {
            return (ImageMoniker)typeof(KnownMonikers).GetProperty($"Method").GetValue(null);
        }

        protected override string GetName()
        {
            return "Methods";
        }
    }
}