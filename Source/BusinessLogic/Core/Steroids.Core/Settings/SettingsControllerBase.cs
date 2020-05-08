using System.Threading.Tasks;
using Steroids.Core.Framework;

namespace Steroids.Core.Settings
{
    public abstract class SettingsControllerBase : ISettingsController
    {
        private readonly IEventAggregator _eventAggregator;

        protected SettingsControllerBase(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator ?? throw new System.ArgumentNullException(nameof(eventAggregator));
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <typeparam name="T">The type of the settings container.</typeparam>
        /// <returns>The current settings object.</returns>
        public abstract Task<T> LoadAsync<T>()
            where T : ISettingsContainer, new();

        /// <summary>
        /// Saves the settings object.
        /// </summary>
        /// <typeparam name="T">The type of the settings container.</typeparam>
        /// <param name="settings">The settings object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveAsync<T>(T settings)
            where T : ISettingsContainer, new()
        {
            await SaveInternalAsync(settings).ConfigureAwait(false);

            var message = new SettingsChangedMessage<T>(settings);
            _eventAggregator.Publish(message);
        }

        protected abstract Task SaveInternalAsync<T>(T settings)
            where T : ISettingsContainer;
    }
}
