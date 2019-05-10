namespace Steroids.Roslyn.CSharp
{
    public class InterfaceNode : CSharpCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Interfaces;

        /// <inheritdoc />
        protected override string NodeTypeName => "Interface";
    }
}