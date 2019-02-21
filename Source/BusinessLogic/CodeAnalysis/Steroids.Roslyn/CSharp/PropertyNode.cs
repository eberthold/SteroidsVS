using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.CSharp
{
    public class PropertyNode : CodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Properties;
    }
}