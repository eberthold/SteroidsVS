using System.Threading.Tasks;
using Microsoft.VisualStudio.Settings;
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
            ISettingsStoreFactory settingsStoreFactory)
            : base(eventAggregator)
        {
            _settingsStore = settingsStoreFactory.Create();
        }

        /// <inheritdoc/>
        public override async Task<T> LoadAsync<T>()
        {
            if (!_loaded)
            {
                await LoadAllAsync().ConfigureAwait(false);
            }

            var container = _settingsContainer.GetSection<T>();
            if (container is null)
            {
                container = new T();
            }

            return (T)container;
        }

        /// <inheritdoc/>
        protected override Task SaveInternalAsync<T>(T settings)
        {
            _settingsContainer.SetSection(settings);

            var json = JsonConvert.SerializeObject(_settingsContainer);
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

            var json = _settingsStore.GetString(CollectionPath, SettingsPropertyName, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
            {
                return Task.CompletedTask;
            }

            _settingsContainer = JsonConvert.DeserializeObject<SteroidsSettingsContainer>(json);
            return Task.CompletedTask;
        }
    }
}
