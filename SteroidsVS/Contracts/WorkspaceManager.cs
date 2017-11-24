namespace SteroidsVS.Contracts
{
    using Microsoft.CodeAnalysis;
    using Steroids.Contracts;

    public class WorkspaceManager : IWorkspaceManager
    {
        public WorkspaceManager(Workspace workspace)
        {
            VsWorkspace = workspace;
        }

        public Workspace VsWorkspace { get; set; }
    }
}
