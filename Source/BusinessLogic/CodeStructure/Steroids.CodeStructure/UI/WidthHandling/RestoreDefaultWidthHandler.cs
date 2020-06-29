using System;
using System.Threading.Tasks;
using Steroids.CodeStructure.Settings;
using Steroids.Core.Framework;
using Steroids.Core.Settings;

namespace Steroids.CodeStructure.UI.WidthHandling
{
    /// <summary>
    /// Handler for restoring the default width currently set in settings.
    /// </summary>
    internal class RestoreDefaultWidthHandler : IWidthHandler
    {
        private readonly ISettingsController _settingsController;
        private double _currentWidth;

        private RestoreDefaultWidthHandler(
            ISettingsController settingsController,
            IEventAggregator eventAggregator)
        {
            _settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController));

            eventAggregator.Subscribe<CodeStructureSettingsContainer>(OnCodeStructureSettingsChanged);
        }

        /// <inheritdoc />
        public event EventHandler<double> CurrentWidthChanged;

        /// <inheritdoc />
        public double GetWidth(string fileName)
            => _currentWidth;

        /// <inheritdoc />
        public void UpdateWidth(double width, string fileName)
        {
            return;
        }

        internal static async Task<IWidthHandler> CreateAsync(
            ISettingsController settingsController,
            IEventAggregator eventAggregator)
        {
            var instance = new RestoreDefaultWidthHandler(settingsController, eventAggregator);
            await instance.LoadAsync().ConfigureAwait(false);
            return instance;
        }

        private async Task LoadAsync()
        {
            var settings = await _settingsController
                .LoadAsync<CodeStructureSettingsContainer>()
                .ConfigureAwait(false);

            _currentWidth = settings.WidthSettings.DefaultWidth;
        }

        private void OnCodeStructureSettingsChanged(CodeStructureSettingsContainer obj)
        {
            _currentWidth = obj.WidthSettings.DefaultWidth;
        }
    }
}
