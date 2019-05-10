namespace Steroids.Roslyn.CSharp
{
    public class StructNode : CSharpCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Structs;

        /// <inheritdoc />
        protected override string NodeTypeName => "Struct";
    }
}