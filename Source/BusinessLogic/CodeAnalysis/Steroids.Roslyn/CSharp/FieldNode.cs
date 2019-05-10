namespace Steroids.Roslyn.CSharp
{
    public class FieldNode : CSharpCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Fields;

        /// <inheritdoc />
        protected override string NodeTypeName => "Field";
    }
}