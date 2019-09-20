namespace Steroids.Roslyn.Common
{
    public class PropertyNode : RoslynCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Properties;

        /// <inheritdoc />
        protected override string NodeTypeName => "Property";
    }
}