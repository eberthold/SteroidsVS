namespace Steroids.CodeStructure.Settings
{
    public class FileWidthInfo
    {
        /// <summary>
        /// Name of the current file.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// The last known width for this file.
        /// </summary>
        public double LastKnownWidth { get; set; } = 250;
    }
}
