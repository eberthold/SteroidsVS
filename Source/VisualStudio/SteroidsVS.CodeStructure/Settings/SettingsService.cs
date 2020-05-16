using System;
using System.Threading.Tasks;
using Steroids.CodeStructure.Settings;
using Steroids.Core.Settings;

namespace SteroidsVS.CodeStructure.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsController _settingsController;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        public SettingsService(ISettingsController settingsController)
        {
            _settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController));
        }

        /// <inheritdoc/>
        public Task<CodeStructureSettingsContainer> LoadSettingsAsync()
        {
            return _settingsController.LoadAsync<CodeStructureSettingsContainer>();
        }

        /// <inheritdoc/>
        public Task SaveSettingsAsync(CodeStructureSettingsContainer settingsContainer)
        {
            return _settingsController.SaveAsync(settingsContainer);
        }
    }
}
