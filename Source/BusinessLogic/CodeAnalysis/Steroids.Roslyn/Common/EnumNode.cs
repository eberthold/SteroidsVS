namespace Steroids.Roslyn.Common
{
    public class EnumNode : RoslynCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Enums;

        /// <inheritdoc />
        protected override string NodeTypeName => "Enumeration";
    }
}