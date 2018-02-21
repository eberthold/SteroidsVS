using Microsoft.VisualStudio.LanguageServices;
using Steroids.Contracts;

namespace SteroidsVS.Contracts
{
    public class WorkspaceManager : IWorkspaceManager
    {
        public WorkspaceManager(VisualStudioWorkspace workspace)
        {
            VsWorkspace = workspace;
        }

        public VisualStudioWorkspace VsWorkspace
        {
            get; set;
        }
    }
}
