using Steroids.Contracts.Core;

namespace Steroids.Core.Framework
{
    public interface IDispatcherServiceFactory
    {
        IDispatcherService Create();
    }
}
