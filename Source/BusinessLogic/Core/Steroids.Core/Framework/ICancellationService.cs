using System.Threading;

namespace Steroids.Core.Framework
{
    public interface ICancellationService
    {
        /// <summary>
        /// Gets a new cancellation token and cancel all other ones created by this service.
        /// </summary>
        CancellationToken GetNewTokenAndCancelOldOnes();
    }
}
