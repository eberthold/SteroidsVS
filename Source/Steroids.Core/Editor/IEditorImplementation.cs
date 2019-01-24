using System.Threading.Tasks;

namespace Steroids.Core.Editor
{
    /// <summary>
    /// Abstraction for editor facades.
    /// </summary>
    public interface IEditorImplementation
    {
        /// <summary>
        /// Path to file which the editor represents.
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// Informs users about folding events in the editor.
        /// </summary>
        IFoldingManager FoldingManager { get; }

        /// <summary>
        /// Gets the raw content which is currently displayed inthe editor.
        /// </summary>
        /// <returns>The current editor content.</returns>
        Task<string> GetRawEditorContentAsync();

        /// <summary>
        /// Calculates the line number after taking collapsed code and other factors into account.
        /// </summary>
        /// <param name="absoluteLineNumber">The absolute line number in code.</param>
        /// <returns>The calculated line number as it is displayed.</returns>
        int GetComputedLineNumber(int absoluteLineNumber);
    }
}
