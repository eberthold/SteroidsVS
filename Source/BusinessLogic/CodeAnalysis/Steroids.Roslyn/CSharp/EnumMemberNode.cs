namespace Steroids.Roslyn.CSharp
{
    public class EnumMemberNode : CSharpCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Enums;

        /// <inheritdoc />
        protected override string NodeTypeName => "EnumerationItem";
    }
}