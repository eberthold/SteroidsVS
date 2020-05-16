using System;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace SteroidsVS.Settings
{
    public class SettingsStoreFactory : ISettingsStoreFactory
    {
        private readonly ShellSettingsManager _settingsManager;

        public SettingsStoreFactory(IServiceProvider serviceProvider)
        {
            _settingsManager = new ShellSettingsManager(serviceProvider);
        }

        public WritableSettingsStore Create()
        {
            return _settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }
    }
}
