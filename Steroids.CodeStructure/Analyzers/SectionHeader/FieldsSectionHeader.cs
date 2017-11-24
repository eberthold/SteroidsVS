namespace Steroids.CodeStructure.Analyzers.SectionHeader
{
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Imaging.Interop;

    public class FieldsSectionHeader : SectionHeaderBase
    {
        public override bool IsMetaNode => true;

        public override int SectionTypeOrderIndex => 5;

        protected override ImageMoniker GetMoniker()
        {
            return (ImageMoniker)typeof(KnownMonikers).GetProperty($"Field").GetValue(null);
        }

        protected override string GetName()
        {
            return "Fields";
        }
    }
}