namespace Steroids.Roslyn.CSharp
{
    public class EnumNode : CSharpCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Enums;

        /// <inheritdoc />
        protected override string NodeTypeName => "Enumeration";
    }
}