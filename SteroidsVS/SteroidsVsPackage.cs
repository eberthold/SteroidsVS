using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using SteroidsVS.Services;
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
    public sealed class SteroidsVsPackage : Package
    {
        public const string PackageGuidString = "9ac11e28-22b5-4c3c-a40f-ab2c9bdd18d6";

        private bool _initialized;

        public IVsServiceProvider VsServiceProvider { get; private set; }

        protected override void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            base.Initialize();

            VsServiceProvider = new VsServiceProvider(this);

            InitializeDictionary<SharedUiModule.Resources.ModuleResourceDictionary>();
            InitializeDictionary<CodeQualityModule.Resources.ModuleResourceDictionary>();
            InitializeDictionary<CodeStructureModule.Resources.ModuleResourceDictionary>();

            var root = new Bootstrapper();
            root.Run(this);
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
