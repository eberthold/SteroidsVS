using Microsoft.VisualStudio.LanguageServices;

namespace Steroids.Contracts
{
    public interface IWorkspaceManager
    {
        VisualStudioWorkspace VsWorkspace { get; }
    }
}
