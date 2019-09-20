namespace Steroids.Roslyn.Common
{
    public class StructNode : RoslynCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Structs;

        /// <inheritdoc />
        protected override string NodeTypeName => "Struct";
    }
}