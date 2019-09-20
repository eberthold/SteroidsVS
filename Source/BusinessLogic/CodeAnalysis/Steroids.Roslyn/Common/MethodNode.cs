namespace Steroids.Roslyn.Common
{
    public class MethodNode : RoslynCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Methods;

        /// <inheritdoc />
        protected override string NodeTypeName => "Method";
    }
}