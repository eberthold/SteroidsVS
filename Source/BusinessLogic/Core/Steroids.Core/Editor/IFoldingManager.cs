using System;

namespace Steroids.Core.Editor
{
    /// <summary>
    /// Notifies about collapsing and expanding events.
    /// </summary>
    public interface IFoldingManager
    {
        /// <summary>
        /// Tells when something was collapsed.
        /// </summary>
        event EventHandler Collapsed;

        /// <summary>
        /// Tells when something was expanded.
        /// </summary>
        event EventHandler Expanded;
    }
}
