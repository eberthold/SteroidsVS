using System.Threading;

namespace Steroids.Core.Framework
{
    public class CancellationService : ICancellationService
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();

        /// <inheritdoc />
        public CancellationToken GetNewTokenAndCancelOldOnes()
        {
            RecreateTokenSource();
            return _cts.Token;
        }

        private void RecreateTokenSource()
        {
            _cts.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }
    }
}
