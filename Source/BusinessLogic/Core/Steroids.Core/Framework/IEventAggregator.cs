using System;

namespace Steroids.Core.Framework
{
    /// <summary>
    /// An interface for an event aggregator.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Subsribe to events of specific type.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="callback">The action to call on the instance.</param>
        void Subscribe<T>(Action<T> callback);

        /// <summary>
        /// Unsibscribe from event.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="callback">The callback to unregister.</param>
        void Unsubscribe<T>(Action<T> callback);

        /// <summary>
        /// Fires the specific event which will provided to all receivers.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="eventArgs">The event args.</param>
        void Publish<T>(T eventArgs);
    }
}
