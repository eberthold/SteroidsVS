using System;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Steroids.Contracts.Core;

namespace SteroidsVS.Services
{
    public class DispatcherService : IDispatcherService
    {
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <inheritdoc />
        public async void Dispatch(Action action)
        {
            ResetTokenSource();
            var token = _tokenSource.Token;

            if (ThreadHelper.CheckAccess())
            {
                Invoke(action, token);
                return;
            }

            await ThreadHelper
                .JoinableTaskFactory
                .SwitchToMainThreadAsync();

            Invoke(action, token);
        }

        private static void Invoke(Action action, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            action.Invoke();
        }

        private void ResetTokenSource()
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = new CancellationTokenSource();
        }
    }
}
