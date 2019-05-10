namespace Steroids.Roslyn.CSharp
{
    /// <summary>
    /// Represents a class in the structure.
    /// </summary>
    public class ClassNode : CSharpCodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Classes;

        /// <inheritdoc />
        protected override string NodeTypeName => "Class";
    }
}