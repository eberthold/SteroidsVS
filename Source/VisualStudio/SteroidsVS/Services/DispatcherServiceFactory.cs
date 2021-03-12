using Steroids.Contracts.Core;
using Steroids.Core.Framework;

namespace SteroidsVS.Services
{
    public class DispatcherServiceFactory : IDispatcherServiceFactory
    {
        public IDispatcherService Create()
        {
            return new DispatcherService();
        }
    }
}
