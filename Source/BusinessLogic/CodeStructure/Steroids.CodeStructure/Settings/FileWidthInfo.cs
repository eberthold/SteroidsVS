namespace Steroids.CodeStructure.Settings
{
    public class FileWidthInfo
    {
        /// <summary>
        /// Path of the current file.
        /// </summary>
        public string FilePath { get; } = string.Empty;

        /// <summary>
        /// The last known width for this file.
        /// </summary>
        public double LastKnownWidth { get; } = 250;

        /// <summary>
        /// Flag to enabled or disable participation in global width syncing.
        /// </summary>
        public bool IsGloballySyncing { get; } = true;
    }
}
