namespace Steroids.CodeStructure.Settings
{
    public enum WidthMode
    {
        /// <summary>
        /// Width uses a default when file is opened.
        /// </summary>
        RestoreDefault,

        /// <summary>
        /// The last used width is stored per file.
        /// </summary>
        StorePerFile,

        /// <summary>
        /// All widths are synced
        /// </summary>
        SyncGlobally
    }
}
