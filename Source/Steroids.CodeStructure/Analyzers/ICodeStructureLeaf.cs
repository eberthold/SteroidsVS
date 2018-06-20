namespace Steroids.CodeStructure.Analyzers
{
    /// <summary>
    /// Interface to mark leafs of the code structure which could not be traversed any more.
    /// </summary>
    public interface ICodeStructureLeaf
    {
        /// <summary>
        /// Gets the name of this node.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the absolute index of this container in a flat list.
        /// </summary>
        int AbsoluteIndex { get; set; }
    }
}
