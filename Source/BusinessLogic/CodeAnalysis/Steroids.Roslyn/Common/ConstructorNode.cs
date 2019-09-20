namespace Steroids.Roslyn.Common
{
    public class ConstructorNode : RoslynCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Constructors;

        /// <inheritdoc />
        protected override string NodeTypeName => "Method";
    }
}