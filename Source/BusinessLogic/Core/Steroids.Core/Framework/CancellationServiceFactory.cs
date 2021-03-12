namespace Steroids.Core.Framework
{
    public class CancellationServiceFactory : ICancellationServiceFactory
    {
        /// <inheritdoc />
        public ICancellationService Create()
        {
            return new CancellationService();
        }
    }
}
