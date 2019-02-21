using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.CSharp
{
    public class EnumNode : CodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Enums;
    }
}