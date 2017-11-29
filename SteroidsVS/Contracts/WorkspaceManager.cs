namespace SteroidsVS.Contracts
{
    using Microsoft.VisualStudio.LanguageServices;
    using Steroids.Contracts;

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
