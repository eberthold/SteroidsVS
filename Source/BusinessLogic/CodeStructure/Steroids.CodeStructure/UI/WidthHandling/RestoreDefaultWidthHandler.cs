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
        private readonly IEventAggregator _eventAggregator;
        private double _currentWidth;

        private RestoreDefaultWidthHandler(
            ISettingsController settingsController,
            IEventAggregator eventAggregator)
        {
            _settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.Subscribe<CodeStructureSettingsContainer>(OnCodeStructureSettingsChanged);
        }

        /// <inheritdoc />
        public event EventHandler<double> CurrentWidthChanged;

        /// <inheritdoc />
        public double CurrentWidth
        {
            get => _currentWidth;
            set
            {
                if (_currentWidth == value)
                {
                    return;
                }

                _currentWidth = value;
                CurrentWidthChanged?.Invoke(this, value);
            }
        }

        internal static async Task<IWidthHandler> CreateAsync(
            ISettingsController settingsController,
            IEventAggregator eventAggregator)
        {
            var instance = new RestoreDefaultWidthHandler(settingsController, eventAggregator);
            await instance.LoadAsync();
            return instance;
        }

        private async Task LoadAsync()
        {
            var settings = await _settingsController
                .LoadAsync<CodeStructureSettingsContainer>()
                .ConfigureAwait(true);

            CurrentWidth = settings.WidthSettings.DefaultWidth;
        }

        private void OnCodeStructureSettingsChanged(CodeStructureSettingsContainer obj)
        {
            CurrentWidth = obj.WidthSettings.DefaultWidth;
        }

        public void UpdateWidth(double width, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
