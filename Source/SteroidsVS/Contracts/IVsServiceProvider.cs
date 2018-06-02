using System.ComponentModel.Design;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.TextManager.Interop;

namespace SteroidsVS
{
    public interface IVsServiceProvider
    {
        IComponentModel ComponentModel { get; }

        IErrorList ErrorList { get; }

        IOutliningManagerService OutliningManagerService { get; }

        IVsTextManager VsTextManager { get; }

        IVsEditorAdaptersFactoryService EditorAdapterFactory { get; }

        IMenuCommandService MenuCommandService { get; }
    }
}
