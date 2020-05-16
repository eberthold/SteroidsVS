using System;
using System.Threading.Tasks;
using Steroids.CodeStructure.Settings;
using Steroids.Core;

namespace SteroidsVS.CodeStructure.Settings
{
    public class SettingsViewModel : BindableBase
    {
        private readonly ISettingsService _settingsService;
        private CodeStructureSettingsContainer _settingsContainer;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// The default width to use for restoring code structure views on document reopen.
        /// </summary>
        public double DefaultRestoreWidth
        {
            get => _settingsContainer.WidthSettings.DefaultWidth;
            set
            {
                _settingsContainer.WidthSettings.DefaultWidth = Math.Max(50, value);
                SaveSettings();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Flag to determine that "restor default" mode is active.
        /// </summary>
        public bool IsRestoreDefault
        {
            get => _settingsContainer.WidthSettings.WidthMode == WidthMode.RestoreWithDefault;
            set
            {
                if (value is false)
                {
                    return;
                }

                _settingsContainer.WidthSettings.WidthMode = WidthMode.RestoreWithDefault;
                SaveSettings();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Flag to determine that "last file width" mode is active.
        /// </summary>
        public bool IsFileBasedRestore
        {
            get => _settingsContainer.WidthSettings.WidthMode == WidthMode.StorePerFile;
            set
            {
                if (value is false)
                {
                    return;
                }

                _settingsContainer.WidthSettings.WidthMode = WidthMode.StorePerFile;
                SaveSettings();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Flag to determine that "syncing all code structure views" is active.
        /// </summary>
        public bool IsSyncGlobally
        {
            get => _settingsContainer.WidthSettings.WidthMode == WidthMode.SyncGlobally;
            set
            {
                if (value is false)
                {
                    return;
                }

                _settingsContainer.WidthSettings.WidthMode = WidthMode.SyncGlobally;
                SaveSettings();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Loads all data asynchronously.
        /// </summary>
        public async Task LoadDataAsync()
        {
            _settingsContainer = await _settingsService.LoadSettingsAsync().ConfigureAwait(false);
        }

        private void SaveSettings()
        {
            _ = SaveSettingsAsync().ConfigureAwait(false);
        }

        private Task SaveSettingsAsync()
        {
            return _settingsService.SaveSettingsAsync(_settingsContainer);
        }
    }
}
