using System;
using System.Threading.Tasks;

namespace Steroids.CodeStructure.UI.WidthHandling
{
    public class GlobalSyncWidthHandler : IWidthHandler
    {
        private double _currentWidth;

        private GlobalSyncWidthHandler()
        {
        }

        /// <inheritdoc />
        public event EventHandler<double> CurrentWidthChanged;

        /// <inheritdoc />
        public double GetWidth(string fileName)
            => _currentWidth;

        /// <inheritdoc />
        public void UpdateWidth(double width, string fileName)
        {
            if (_currentWidth == width)
            {
                return;
            }

            CurrentWidthChanged?.Invoke(this, width);
            _currentWidth = width;
        }

        internal static Task<GlobalSyncWidthHandler> CreateAsync()
        {
            return Task.FromResult(new GlobalSyncWidthHandler());
        }
    }
}
