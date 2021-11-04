using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SteroidsVS.Services;
using CodeQualityModule = SteroidsVS.CodeQuality;
using CodeStructureModule = SteroidsVS.CodeStructure;
using SharedUiModule = Steroids.SharedUI;
using Threading = System.Threading.Tasks;

namespace SteroidsVS
{
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("SteroidsVS", "Multi-purpose extension for VisualStudio", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideBindingPath]
    public sealed class SteroidsVsPackage : AsyncPackage
    {
        public const string PackageGuidString = "9ac11e28-22b5-4c3c-a40f-ab2c9bdd18d6";

        public static readonly Guid PackageGuid = Guid.Parse("9ac11e28-22b5-4c3c-a40f-ab2c9bdd18d6");

        private static TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();

        private bool _initialized;

        public static Threading.Task InitializedAwaitable => _taskCompletionSource.Task;

        /// <summary>
        /// Checks if this <see cref="Package"/> has been loaded, and loads it asynchronously if not loaded.
        /// </summary>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Threading.Task EnsurePackageLoadedAsync()
        {
            Debug.WriteLine("Steroids: EnsurePackageLoadedAsync - entered");
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            Debug.WriteLine("Steroids: EnsurePackageLoadedAsync - resolve global services");
            var shell = GetGlobalService(typeof(SVsShell)) as IVsShell;
            var shell7 = GetGlobalService(typeof(SVsShell)) as IVsShell7;

            Debug.WriteLine("Steroids: EnsurePackageLoadedAsync - check package loaded");
            if (shell.IsPackageLoaded(PackageGuid, out _) == VSConstants.S_OK)
            {
                Debug.WriteLine("Steroids: EnsurePackageLoadedAsync - package loaded");
                return;
            }

            Debug.WriteLine("Steroids: EnsurePackageLoadedAsync - do load package");
            await shell7.LoadPackageAsync(PackageGuid);
            Debug.WriteLine("Steroids: EnsurePackageLoadedAsync - done");
        }

        /// <inheritdoc />
        protected async override Threading.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Debug.WriteLine("Steroids: InitializeAsync - entered");
            if (_initialized)
            {
                Debug.WriteLine("Steroids: InitializeAsync - already initialized");
                return;
            }

            _initialized = true;

            Debug.WriteLine("Steroids: InitializeAsync - step 1");
            ReportNextInitStep(progress, 1);
            await base.InitializeAsync(cancellationToken, progress).ConfigureAwait(false);

            Debug.WriteLine("Steroids: InitializeAsync - step 2");
            ReportNextInitStep(progress, 2);
            InitializeResources();

            Debug.WriteLine("Steroids: InitializeAsync - step 3");
            ReportNextInitStep(progress, 3);

            var vsServiceProvider = new VsServiceProvider(this);
            Debug.WriteLine("Steroids: InitializeAsync - Initialize VS Service Provider");
            await vsServiceProvider.InitializeAsync().ConfigureAwait(false);

            Debug.WriteLine("Steroids: InitializeAsync - Run Bootstrapper");
            var root = new Bootstrapper();
            root.Run(vsServiceProvider);

            Debug.WriteLine("Steroids: InitializeAsync - Done");
            _taskCompletionSource.SetResult(true);
        }

        /// <summary>
        /// Reports the current step of initialization.
        /// </summary>
        /// <param name="progress">The <see cref="IProgress{ServiceProgressData}"/> channel.</param>
        /// <param name="step">The number of the current step.</param>
        private static void ReportNextInitStep(IProgress<ServiceProgressData> progress, int step)
        {
            var progressData = new ServiceProgressData("Starting SteroidsVS", "Initializing...", step, 3);
            progress.Report(progressData);
        }

        /// <summary>
        /// Initializes all <see cref="ResourceDictionary"/> instances of used submodules.
        /// </summary>
        private static void InitializeResources()
        {
            InitializeDictionary<SharedUiModule.Resources.ModuleResourceDictionary>();
            InitializeDictionary<CodeQualityModule.Resources.ModuleResourceDictionary>();
            InitializeDictionary<CodeStructureModule.Resources.ModuleResourceDictionary>();
        }

        /// <summary>
        /// Loads a specific <see cref="ResourceDictionary"/> into the resource tree.
        /// </summary>
        /// <typeparam name="T">The type of the ResourceDictionary.</typeparam>
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
