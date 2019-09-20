namespace Steroids.Roslyn.Common
{
    public class EnumMemberNode : RoslynCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Enums;

        /// <inheritdoc />
        protected override string NodeTypeName => "EnumerationItem";
    }
}