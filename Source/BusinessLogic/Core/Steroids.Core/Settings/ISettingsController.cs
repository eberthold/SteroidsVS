using System.Threading.Tasks;

namespace Steroids.Core.Settings
{
    public interface ISettingsController
    {
        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <typeparam name="T">The type of the settings container.</typeparam>
        /// <returns>The current settings object.</returns>
        Task<T> LoadAsync<T>()
            where T : ISettingsContainer;

        /// <summary>
        /// Saves the settings object.
        /// </summary>
        /// <typeparam name="T">The type of the settings container.</typeparam>
        /// <param name="settings">The settings object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveAsync<T>(T settings)
            where T : ISettingsContainer;
    }
}