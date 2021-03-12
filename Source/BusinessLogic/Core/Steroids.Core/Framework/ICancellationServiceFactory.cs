namespace Steroids.Core.Framework
{
    public interface ICancellationServiceFactory
    {
        /// <summary>
        /// Creates a new <see cref="ICancellationService"/>.
        /// </summary>
        ICancellationService Create();
    }
}
