using System.ComponentModel;

namespace Steroids.CodeStructure.UI.WidthHandling
{
    public interface IWidthTarget : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the current width value.
        /// </summary>
        double Width { get; set; }
    }
}
