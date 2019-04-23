namespace Steroids.Roslyn.CSharp
{
    public class PropertyNode : CSharpCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Properties;

        /// <inheritdoc />
        protected override string NodeTypeName => "Property";
    }
}