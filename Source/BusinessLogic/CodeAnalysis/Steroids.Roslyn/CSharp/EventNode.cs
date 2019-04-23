namespace Steroids.Roslyn.CSharp
{
    public class EventNode : CSharpCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Events;

        /// <inheritdoc />
        protected override string NodeTypeName => "Event";
    }
}
