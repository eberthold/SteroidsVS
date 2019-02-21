using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.CSharp
{
    public class EventNode : CodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Events;
    }
}
