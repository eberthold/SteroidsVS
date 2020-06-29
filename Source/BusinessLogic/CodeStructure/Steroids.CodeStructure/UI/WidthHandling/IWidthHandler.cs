using System;

namespace Steroids.CodeStructure.UI.WidthHandling
{
    /// <summary>
    /// Handle width changes of the code structure in both directions.
    /// </summary>
    public interface IWidthHandler
    {
        /// <summary>
        /// Raises an event, that the current width has changed.
        /// </summary>
        event EventHandler<double> CurrentWidthChanged;

        /// <summary>
        /// Gets the current width for given file.
        /// </summary>
        double GetWidth(string fileName);

        /// <summary>
        /// Updates the <see cref="GetWidth"/> and provides the name of the current file.
        /// </summary>
        /// <param name="width">The width value.</param>
        /// <param name="fileName">The name of the file.</param>
        void UpdateWidth(double width, string fileName);
    }
}
