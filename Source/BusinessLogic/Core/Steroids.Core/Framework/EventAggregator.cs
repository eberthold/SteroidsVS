using System;
using System.Collections.Generic;
using System.Linq;

namespace Steroids.Core.Framework
{
    /// <summary>
    /// Simple event aggregator to fire events application wide to subscribers.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly List<EventReceiverContainer> _receiverContainers = new List<EventReceiverContainer>();

        /// <inheritdoc />
        public void Publish<T>(T eventArgs)
        {
            var container = GetOrAddContainer<T>();
            foreach (var receiver in container.GetLivingReceivers<T>())
            {
                receiver(eventArgs);
            }
        }

        /// <inheritdoc />
        public void Subscribe<T>(Action<T> callback)
        {
            var targetHash = GetCallbackHash(callback);
            var container = GetOrAddContainer<T>();
            container.AddReceiver(targetHash, callback);
        }

        /// <inheritdoc />
        public void Unsubscribe<T>(Action<T> callback)
        {
            var targetHash = GetCallbackHash(callback);
            var container = GetOrAddContainer<T>();
            container.RemoveReceiver(targetHash);
        }

        /// <summary>
        /// Only for unit test reasons.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <returns>The count of living subscribers for this message type.</returns>
        internal int GetSubscriberCountFor<T>()
        {
            var container = GetOrAddContainer<T>();
            return container.GetReceiversCount();
        }

        /// <summary>
        /// Calculates the hash code to uniquely indentify callback method.
        /// </summary>
        /// <returns>The hash code.</returns>
        private static int GetCallbackHash<T>(Action<T> callback)
        {
            return callback.Target.GetHashCode() + callback.Method.Name.GetHashCode();
        }

        private EventReceiverContainer GetOrAddContainer<T>()
        {
            var container = _receiverContainers.Find(x => x.EventType == typeof(T));
            if (container is object)
            {
                return container;
            }

            container = new EventReceiverContainer(typeof(T));
            _receiverContainers.Add(container);
            return container;
        }

        /// <summary>
        /// Helper class to keep weak references of subscribers.
        /// </summary>
        private class EventReceiverContainer
        {
            private readonly Dictionary<int, WeakReference> _receivers = new Dictionary<int, WeakReference>();

            public EventReceiverContainer(Type type)
            {
                EventType = type;
            }

            internal Type EventType { get; }

            public void AddReceiver(int targetHash, object receiver)
            {
                if (_receivers.ContainsKey(targetHash))
                {
                    return;
                }

                _receivers.Add(targetHash, new WeakReference(receiver));
            }

            public void RemoveReceiver(int targetHash)
            {
                if (!_receivers.ContainsKey(targetHash))
                {
                    return;
                }

                var weakReference = _receivers[targetHash];
                if (weakReference is null)
                {
                    return;
                }

                _receivers.Remove(targetHash);
            }

            public IReadOnlyCollection<Action<T>> GetLivingReceivers<T>()
            {
                RemoveDeads();

                return _receivers
                    .Select(x => x.Value.Target)
                    .OfType<Action<T>>()
                    .ToList();
            }

            internal int GetReceiversCount()
            {
                RemoveDeads();
                return _receivers.Count;
            }

            private void RemoveDeads()
            {
                var deadReceivers = _receivers.Where(x => !x.Value.IsAlive).ToList();
                foreach (var deadReceiver in deadReceivers)
                {
                    _receivers.Remove(deadReceiver.Key);
                }
            }
        }
    }
}
