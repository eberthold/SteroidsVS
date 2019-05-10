using System;

namespace Steroids.Core.UI
{
    public interface IAdornmentSpaceReservation
    {
        /// <summary>
        /// Will be raised, when the <see cref="ActualWidth"/> has changed.
        /// </summary>
        event EventHandler ActualWidthChanged;

        /// <summary>
        /// Gets where the reservation is located.
        /// </summary>
        ReservationLocation Location { get; }

        /// <summary>
        /// Gets or sets the current actual width of the reservation.
        /// </summary>
        double ActualWidth { get; set; }
    }
}
