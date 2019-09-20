namespace Steroids.Roslyn.Common
{
    public class InterfaceNode : RoslynCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Interfaces;

        /// <inheritdoc />
        protected override string NodeTypeName => "Interface";
    }
}