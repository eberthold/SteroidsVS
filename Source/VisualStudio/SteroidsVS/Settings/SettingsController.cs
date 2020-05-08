using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using Newtonsoft.Json;
using Steroids.Core.Framework;
using Steroids.Core.Settings;

namespace SteroidsVS.Settings
{
    public class SettingsController : SettingsControllerBase
    {
        private const string CollectionPath = "Steroids_7EB2E528-DFFF-431D-A6C0-2274047FCB7A";
        private const string SettingsPropertyName = "Settings";
        private readonly WritableSettingsStore _settingsStore;

        private SteroidsSettingsContainer _settingsContainer = new SteroidsSettingsContainer();
        private bool _loaded = false;

        public SettingsController(
            IEventAggregator eventAggregator,
            IServiceProvider serviceProvider)
            : base(eventAggregator)
        {
            var settingsManager = new ShellSettingsManager(serviceProvider);
            _settingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        public override async Task<T> LoadAsync<T>()
        {
            if (!_loaded)
            {
                await LoadAllAsync().ConfigureAwait(false);
            }

            var container = _settingsContainer.Containers.First(x => x.GetType() == typeof(T));
            if (container is null)
            {
                container = new T();
            }

            return (T)container;
        }

        protected override Task SaveInternalAsync<T>(T settings)
        {
            var index = _settingsContainer.Containers.FindIndex(x => x.Key == settings.Key);
            _settingsContainer.Containers[index] = settings;

            var json = JsonConvert.SerializeObject(_settingsContainer.Containers);
            _settingsStore.SetString(CollectionPath, SettingsPropertyName, json);
            return Task.CompletedTask;
        }

        private Task LoadAllAsync()
        {
            if (!_settingsStore.CollectionExists(CollectionPath))
            {
                _settingsStore.CreateCollection(CollectionPath);
                return Task.CompletedTask;
            }

            var json = _settingsStore.GetString(CollectionPath, SettingsPropertyName);
            if (string.IsNullOrWhiteSpace(json))
            {
                return Task.CompletedTask;
            }

            _settingsContainer.Containers = JsonConvert.DeserializeObject<List<ISettingsContainer>>(json);
            return Task.CompletedTask;
        }
    }
}
