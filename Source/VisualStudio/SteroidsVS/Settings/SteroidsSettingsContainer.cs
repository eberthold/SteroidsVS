using System.Collections.Generic;
using Steroids.Core.Settings;

namespace SteroidsVS.Settings
{
    public class SteroidsSettingsContainer : ISettingsContainer
    {
        /// <inheritdoc />
        public string Key { get; } = "F7CB3653-0B39-4D0A-BF4F-C8C7AAD41580";

        /// <summary>
        /// All the settings.
        /// </summary>
        public List<ISettingsContainer> Containers { get; set; }
    }
}
