using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Steroids.Core.Framework
{
    public class EventAggregator : IEventAggregator
    {
        private List<EventReceiverContainer> _receiverContainers = new List<EventReceiverContainer>();

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
            container.RemoveReceiver(targetHash, callback);
        }

        internal int GetSubscriberCountFor<T>()
        {
            var container = GetOrAddContainer<T>();
            return container.GetReceiversCount();
        }

        private static int GetCallbackHash<T>(Action<T> callback)
        {
            return callback.Target.GetHashCode() + callback.Method.Name.GetHashCode();
        }

        private EventReceiverContainer GetOrAddContainer<T>()
        {
            var container = _receiverContainers.FirstOrDefault(x => x.EventType == typeof(T));
            if (container is object)
            {
                return container;
            }

            container = new EventReceiverContainer(typeof(T));
            _receiverContainers.Add(container);
            return container;
        }

        private class EventReceiverContainer
        {
            private Dictionary<int, WeakReference> _receivers = new Dictionary<int, WeakReference>();

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

            public void RemoveReceiver(int targetHash, object receiver)
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
