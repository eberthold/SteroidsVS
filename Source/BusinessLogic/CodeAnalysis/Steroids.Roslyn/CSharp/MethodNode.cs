namespace Steroids.Roslyn.CSharp
{
    public class MethodNode : CSharpCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Methods;

        /// <inheritdoc />
        protected override string NodeTypeName => "Method";
    }
}