using Steroids.Core.Settings;

namespace Steroids.CodeStructure.Settings
{
    public class CodeStructureSettingsContainer : ISettingsContainer
    {
        /// <inheritdoc />
        public string Key { get; } = "4160217C-9255-4E50-AB37-10E5B0CF2B1B";

        /// <summary>
        /// The width settings for the CodeStructure.
        /// </summary>
        public CodeStructureWidthSettings WidthSettings { get; internal set; }
    }
}
