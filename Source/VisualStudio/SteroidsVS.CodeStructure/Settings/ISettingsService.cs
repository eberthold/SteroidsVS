using System.Threading.Tasks;
using Steroids.CodeStructure.Settings;

namespace SteroidsVS.CodeStructure.Settings
{
    public interface ISettingsService
    {
        /// <summary>
        /// Loads the settings asynchronously.
        /// </summary>
        Task<CodeStructureSettingsContainer> LoadSettingsAsync();

        /// <summary>
        /// Saves the settings asynchronously.
        /// </summary>
        Task SaveSettingsAsync(CodeStructureSettingsContainer settingsContainer);
    }
}
