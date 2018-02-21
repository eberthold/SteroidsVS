using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;

namespace SteroidsVS
{
    public interface IVsPackageServices
    {
        VisualStudioWorkspace Workspace { get; }

        IErrorList ErrorList { get; }
    }
}
