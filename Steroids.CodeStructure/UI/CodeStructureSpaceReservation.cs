using System;
using Steroids.Contracts.UI;

namespace Steroids.CodeStructure.UI
{
    public class CodeStructureSpaceReservation : IAdornmentSpaceReservation
    {
        private double _actualWidth;

        /// <inheritdoc />
        public event EventHandler ActualWidthChanged;

        /// <inheritdoc />
        public ReservationLocation Location => ReservationLocation.CodeStructure;

        /// <inheritdoc />
        public double ActualWidth
        {
            get
            {
                return _actualWidth;
            }

            set
            {
                if (_actualWidth == value)
                {
                    return;
                }

                _actualWidth = value;
                ActualWidthChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
