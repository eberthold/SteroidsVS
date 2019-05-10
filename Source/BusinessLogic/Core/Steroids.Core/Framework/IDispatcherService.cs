using System;

namespace Steroids.Contracts.Core
{
    /// <summary>
    /// Service for dispatching execution to the main thread.
    /// </summary>
    public interface IDispatcherService
    {
        /// <summary>
        /// Dispatches the <paramref name="action"/> to the main thread.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to dispatch.</param>
        void Dispatch(Action action);
    }
}
