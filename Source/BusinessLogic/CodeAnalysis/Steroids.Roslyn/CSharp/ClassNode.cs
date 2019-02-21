using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.CSharp
{
    /// <summary>
    /// Represents a class in the structure.
    /// </summary>
    public class ClassNode : CodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Classes;
    }
}