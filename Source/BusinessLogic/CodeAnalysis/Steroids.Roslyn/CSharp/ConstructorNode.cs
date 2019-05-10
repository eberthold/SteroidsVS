namespace Steroids.Roslyn.CSharp
{
    public class ConstructorNode : CSharpCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Constructors;

        /// <inheritdoc />
        protected override string NodeTypeName => "Method";
    }
}