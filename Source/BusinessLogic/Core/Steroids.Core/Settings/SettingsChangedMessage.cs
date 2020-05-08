namespace Steroids.Core.Settings
{
    public class SettingsChangedMessage<T>
        where T : ISettingsContainer
    {
        public SettingsChangedMessage(T settingsContainer)
        {
            SettingsContainer = settingsContainer;
        }

        /// <summary>
        /// The <see cref="SettingsContainer"/> which has changed.
        /// </summary>
        public T SettingsContainer { get; }
    }
}
