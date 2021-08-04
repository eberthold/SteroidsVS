using Microsoft.VisualStudio.Text.Editor;

namespace SteroidsVS.Services
{
    public interface IActiveTextViewProvider
    {
        IWpfTextView ActiveTextView { get; }
    }
}
