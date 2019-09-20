namespace Steroids.Roslyn.Common
{
    /// <summary>
    /// Represents a class in the structure.
    /// </summary>
    public class ClassNode : RoslynCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Classes;

        /// <inheritdoc />
        protected override string NodeTypeName => "Class";
    }
}