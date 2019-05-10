namespace Steroids.Core.Framework
{
    /// <summary>
    /// Provides the entry point for modules.
    /// </summary>
    public interface IModuleInitializer
    {
        /// <summary>
        /// Does the initialization of the module.
        /// </summary>
        void Initialize();
    }
}
