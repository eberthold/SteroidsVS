namespace Steroids.Core.CodeQuality
{
    public enum DiagnosticSeverity
    {
        /// <summary>
        /// Diagnostic which isn't diaplayed.
        /// </summary>
        Hidden = 0,

        /// <summary>
        /// An information which might improve the code.
        /// </summary>
        Info = 1,

        /// <summary>
        /// A warning may causes problems at runtime.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error which prevents project from compiling.
        /// </summary>
        Error = 3
    }
}
