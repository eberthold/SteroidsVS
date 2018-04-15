using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.TextManager.Interop;

namespace SteroidsVS
{
    public interface IVsPackageServices
    {
        VisualStudioWorkspace Workspace { get; }

        IErrorList ErrorList { get; }

        IOutliningManagerService OutliningManagerService { get; }

        /// <summary>
        /// Gets the <see cref="IVsTextManager"/>.
        /// </summary>
        IVsTextManager VsTextManager { get; }
    }
}
