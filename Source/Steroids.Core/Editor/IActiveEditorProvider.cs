namespace Steroids.Core.Editor
{
    public interface IActiveEditorProvider
    {
        /// <summary>
        /// Gets the active text view or null.
        /// </summary>
        /// <returns>The <see cref="IActiveEditorProvider"/> or <see langword="null"/> if none is active.</returns>
        IEditor GetActiveEditor();
    }
}
