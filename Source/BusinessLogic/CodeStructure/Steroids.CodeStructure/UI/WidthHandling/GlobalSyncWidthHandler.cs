using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steroids.CodeStructure.UI.WidthHandling
{
    public class GlobalSyncWidthHandler : IWidthHandler
    {
        private double _currentWidth;

        /// <inheritdoc />
        public event EventHandler<double> CurrentWidthChanged;

        /// <inheritdoc />
        public double CurrentWidth
        {
            get => _currentWidth;
            set
            {
                if (_currentWidth == value)
                {
                    return;
                }

                CurrentWidthChanged?.Invoke(this, value);
                _currentWidth = value;
            }
        }

        public void UpdateWidth(double width, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
