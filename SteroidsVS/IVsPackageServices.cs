using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Outlining;

namespace SteroidsVS
{
    public interface IVsPackageServices
    {
        VisualStudioWorkspace Workspace { get; }

        IErrorList ErrorList { get; }

        IOutliningManagerService OutliningManagerService { get; }
    }
}
