using System;

namespace Steroids.Contracts.Core
{
    public interface IDispatcherService
    {
        /// <summary>
        /// Dispacthes the <paramref name="action"/> to the UI thread.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to dispatch.</param>
        void Dispatch(Action action);
    }
}
