using Microsoft.CodeAnalysis;

namespace Steroids.Contracts
{
    public interface IWorkspaceManager
    {
        Workspace VsWorkspace { get; }
    }
}
