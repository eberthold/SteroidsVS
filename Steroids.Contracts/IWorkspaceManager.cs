namespace Steroids.Contracts
{
    using Microsoft.VisualStudio.LanguageServices;

    public interface IWorkspaceManager
    {
        VisualStudioWorkspace VsWorkspace { get; }
    }
}
