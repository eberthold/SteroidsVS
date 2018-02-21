using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Steroids.CodeStructure.Analyzers.SectionHeader
{
    public class PropertiesSectionHeader : SectionHeaderBase
    {
        public override bool IsMetaNode => true;

        public override int SectionTypeOrderIndex => 8;

        protected override ImageMoniker GetMoniker()
        {
            return (ImageMoniker)typeof(KnownMonikers).GetProperty($"Property").GetValue(null);
        }

        protected override string GetName()
        {
            return "Properties";
        }
    }
}