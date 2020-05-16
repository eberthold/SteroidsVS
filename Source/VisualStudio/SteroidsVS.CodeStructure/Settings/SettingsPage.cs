using System.Threading.Tasks;
using System.Windows;
using Shell = Microsoft.VisualStudio.Shell;

namespace SteroidsVS.CodeStructure.Settings
{
    public class SettingsPage : Shell.UIElementDialogPage
    {
        public const string SettingsPageName = "CodeStructure";

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPage"/> class.
        /// </summary>
        public SettingsPage()
        {
            var view = new SettingsView();
            _ = InitializeAsync(view).ConfigureAwait(true);

            Child = view;
        }

        /// <inheritdoc/>
        protected override UIElement Child { get; }

        /// <summary>
        /// Initializes the view asynchronously.
        /// </summary>
        /// <remarks>
        /// This must be done by using ServiceLocator, because instanciation of this is done by VisualStudio.
        /// </remarks>
        private async Task InitializeAsync(SettingsView view)
        {
            var vm = CommonServiceLocator.ServiceLocator.Current.GetInstance<SettingsViewModel>();
            await vm.LoadDataAsync().ConfigureAwait(true);
            view.DataContext = vm;
        }
    }
}
