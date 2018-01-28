namespace SteroidsVS
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Steroids.CodeStructure;

    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    public sealed class SteroidsVsPackage : Package, IVsPackageServices
    {
        public const string PackageGuidString = "9ac11e28-22b5-4c3c-a40f-ab2c9bdd18d6";

        private bool _initialized;

        public VisualStudioWorkspace Workspace
        {
            get; private set;
        }

        public IErrorList ErrorList
        {
            get; private set;
        }

        protected override void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            base.Initialize();

            CodeStructureInitializer.Initialize();

            var componentModel = (IComponentModel)GetGlobalService(typeof(SComponentModel));
            Workspace = componentModel.GetService<VisualStudioWorkspace>();
            ErrorList = GetService(typeof(SVsErrorList)) as IErrorList;

            var root = new Bootstrapper();
            root.Initialize(this);
        }
    }
}
