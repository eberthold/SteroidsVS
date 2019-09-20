namespace Steroids.Roslyn.Common
{
    public class EventNode : RoslynCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Events;

        /// <inheritdoc />
        protected override string NodeTypeName => "Event";
    }
}
