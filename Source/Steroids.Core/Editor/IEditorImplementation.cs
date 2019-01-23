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
        /// Gets the raw content which is currently displayed inthe editor.
        /// </summary>
        /// <returns>The current editor content.</returns>
        Task<string> GetRawEditorContentAsync();
    }
}
