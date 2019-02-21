using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.CSharp
{
    public class StructNode : CodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Structs;
    }
}