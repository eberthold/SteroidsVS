using System;
using Microsoft.VisualStudio.Shell;
using Steroids.Contracts.Core;
using Threading = System.Threading.Tasks;

namespace SteroidsVS.Services
{
    public class DispatcherService : IDispatcherService
    {
        /// <inheritdoc />
        public void Dispatch(Action action)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(() =>
            {
                action.Invoke();
                return Threading.Task.CompletedTask;
            });
        }
    }
}
