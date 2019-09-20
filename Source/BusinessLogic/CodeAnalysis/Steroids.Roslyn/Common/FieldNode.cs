namespace Steroids.Roslyn.Common
{
    public class FieldNode : RoslynCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Fields;

        /// <inheritdoc />
        protected override string NodeTypeName => "Field";
    }
}