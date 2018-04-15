using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.TextManager.Interop;
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
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class SteroidsVsPackage : Package, IVsPackageServices
    {
        public const string PackageGuidString = "9ac11e28-22b5-4c3c-a40f-ab2c9bdd18d6";

        private bool _initialized;

        public IComponentModel ComponentModel { get; private set; }

        /// <inheritdoc />
        public VisualStudioWorkspace Workspace { get; private set; }

        /// <inheritdoc />
        public IErrorList ErrorList { get; private set; }

        /// <inheritdoc />
        public IOutliningManagerService OutliningManagerService { get; private set; }

        /// <inheritdoc />
        public IVsTextManager VsTextManager { get; private set; }

        public IVsEditorAdaptersFactoryService EditorAdapterFactory { get; private set; }

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

            InitializeStudioServices();

            var root = new Bootstrapper();
            root.Run(this);
        }

        private void InitializeStudioServices()
        {
            ComponentModel = (IComponentModel)GetGlobalService(typeof(SComponentModel));
            Workspace = ComponentModel.GetService<VisualStudioWorkspace>();
            OutliningManagerService = ComponentModel.GetService<IOutliningManagerService>();
            EditorAdapterFactory = ComponentModel.GetService<IVsEditorAdaptersFactoryService>();

            ErrorList = GetService(typeof(SVsErrorList)) as IErrorList;
            VsTextManager = GetService(typeof(SVsTextManager)) as IVsTextManager;
        }

        /// <summary>
        /// Loads a specific <see cref="ResourceDictionary"/> into the resource tree.
        /// </summary>
        /// <typeparam name="T">The tzpe of the ResourceDictionary.</typeparam>
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
