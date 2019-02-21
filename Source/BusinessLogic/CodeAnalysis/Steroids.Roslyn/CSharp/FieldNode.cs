using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.CSharp
{
    public class FieldNode : CodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Fields;
    }
}