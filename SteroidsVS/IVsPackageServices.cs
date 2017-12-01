namespace SteroidsVS
{
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Shell;

    public interface IVsPackageServices
    {
        VisualStudioWorkspace Workspace { get; }

        IErrorList ErrorList { get; }
    }
}
