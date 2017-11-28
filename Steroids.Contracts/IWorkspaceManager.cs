namespace Steroids.Contracts
{
    using Microsoft.CodeAnalysis;

    public interface IWorkspaceManager
    {
        Workspace VsWorkspace { get; }
    }
}
