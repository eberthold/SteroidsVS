using System;
using System.Windows;
using System.Windows.Threading;
using Steroids.Contracts.Core;

namespace SteroidsVS.Services
{
    public class DispatcherService : IDispatcherService
    {
        private Dispatcher _dispatcher = Application.Current.Dispatcher;

        /// <inheritdoc />
        public void Dispatch(Action action)
        {
            _dispatcher.Invoke(action);
        }
    }
}
