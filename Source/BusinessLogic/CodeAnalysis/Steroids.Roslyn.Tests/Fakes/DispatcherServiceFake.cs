using System;
using Steroids.Contracts.Core;

namespace Steroids.Roslyn.Tests.Fakes
{
    public class DispatcherServiceFake : IDispatcherService
    {
        /// <summary>
        /// Fakes the service and directly invokes the given action.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to invoke.</param>
        public void Dispatch(Action action)
        {
            action.Invoke();
        }
    }
}
