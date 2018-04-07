using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Outlining;
using CodeQualityModule = Steroids.CodeQuality;
using CodeStructureModule = Steroids.CodeStructure;
using SharedUiModule = Steroids.SharedUI;

namespace SteroidsVS
{
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    public sealed class SteroidsVsPackage : Package, IVsPackageServices
    {
        public const string PackageGuidString = "9ac11e28-22b5-4c3c-a40f-ab2c9bdd18d6";

        private bool _initialized;

        /// <inheritdoc />
        public VisualStudioWorkspace Workspace { get; private set; }

        /// <inheritdoc />
        public IErrorList ErrorList { get; private set; }

        /// <inheritdoc />
        public IOutliningManagerService OutliningManagerService { get; private set; }

        protected override void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            base.Initialize();

            InitializeDictionary<SharedUiModule.Resources.ModuleResourceDictionary>();
            InitializeDictionary<CodeQualityModule.Resources.ModuleResourceDictionary>();
            InitializeDictionary<CodeStructureModule.Resources.ModuleResourceDictionary>();

            var componentModel = (IComponentModel)GetGlobalService(typeof(SComponentModel));
            Workspace = componentModel.GetService<VisualStudioWorkspace>();
            ErrorList = GetService(typeof(SVsErrorList)) as IErrorList;
            OutliningManagerService = componentModel.GetService<IOutliningManagerService>();

            var root = new Bootstrapper();
            root.Initialize(this);
        }

        private static void InitializeDictionary<T>()
            where T : ResourceDictionary, new()
        {
            if (!Application.Current.Resources.MergedDictionaries.OfType<T>().Any())
            {
                Application.Current.Resources.MergedDictionaries.Add(new T());
            }
        }
    }
}
