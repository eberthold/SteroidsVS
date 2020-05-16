using Microsoft.VisualStudio.Settings;

namespace SteroidsVS.Settings
{
    /// <summary>
    /// Factory for providing the <see cref="WritableSettingsStore"/>.
    /// Mainly used for faking in UnitTest.
    /// </summary>
    public interface ISettingsStoreFactory
    {
        /// <summary>
        /// Creates the <see cref="WritableSettingsStore"/> to use.
        /// </summary>
        WritableSettingsStore Create();
    }
}