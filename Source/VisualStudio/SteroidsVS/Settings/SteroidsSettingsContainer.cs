using Steroids.CodeStructure.Settings;
using Steroids.Core.Settings;

namespace SteroidsVS.Settings
{
    public class SteroidsSettingsContainer : ISettingsContainer
    {
        /// <inheritdoc />
        public string Key { get; } = "F7CB3653-0B39-4D0A-BF4F-C8C7AAD41580";

        /// <summary>
        /// The settings for code structure.
        /// </summary>
        public CodeStructureSettingsContainer CodeStructure { get; set; }

        /// <summary>
        /// Get the settings section representing the specfici type.
        /// </summary>
        public ISettingsContainer GetSection<T>()
            where T : ISettingsContainer, new()
        {
            if (typeof(T) == typeof(CodeStructureSettingsContainer))
            {
                return CodeStructure;
            }

            return new T();
        }

        /// <summary>
        /// Sets the specific settings section which matches the given type.
        /// </summary>
        public void SetSection(ISettingsContainer settings)
        {
            if (settings.GetType() == typeof(CodeStructureSettingsContainer))
            {
                CodeStructure = (CodeStructureSettingsContainer)settings;
            }
        }
    }
}
