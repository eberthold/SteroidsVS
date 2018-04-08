using Microsoft.VisualStudio.Text.Editor;

namespace Steroids.Core.Contracts
{
    public interface IQualityTextView
    {
        /// <summary>
        /// Gets the textview, which provides all the Data.
        /// </summary>
        IWpfTextView TextView { get; }

        /// <summary>
        /// Gets the Path for the TextView.
        /// </summary>
        string Path { get; }
    }
}
