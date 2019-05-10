using System;
using System.Threading.Tasks;

namespace Steroids.Core.Editor
{
    /// <summary>
    /// Abstraction for editor facades.
    /// </summary>
    public interface IEditor
    {
        /// <summary>
        /// Fired when the content of the editor has changed.
        /// </summary>
        /// <remarks>
        /// Should not be triggered if the editor is not visible to the user.
        /// </remarks>
        event EventHandler ContentChanged;

        /// <summary>
        /// Gets the type of the content e.g. "CSharp".
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Path to file which the editor represents.
        /// </summary>
        string FilePath { get; }

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

        /// <summary>
        /// Sets the cursor to the beginning of the specific line.
        /// </summary>
        /// <param name="absoluteLineNumber">The absolute line number in code.</param>
        void SetCursorToLine(int absoluteLineNumber);
    }
}
