using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Steroids.SharedUI.Behaviors
{
    /// <summary>
    /// This behavior handles mouse over events to show pop-ups as interactive tool tip.
    /// </summary>
    public static class MouseOverPopupBehavior
    {
        public static readonly DependencyProperty PopupProperty =
            DependencyProperty.RegisterAttached("Popup", typeof(Popup), typeof(MouseOverPopupBehavior), new PropertyMetadata(null, OnPopupChanged));

        internal static readonly DependencyProperty ControlProperty =
            DependencyProperty.RegisterAttached("Control", typeof(FrameworkElement), typeof(MouseOverPopupBehavior), new PropertyMetadata(null));

        private static readonly Size DefaultTolerance = new Size(5, 5);

        /// <summary>
        /// Getter function for <see cref="PopupProperty"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>.</param>
        /// <returns>The <see cref="Popup"/>.</returns>
        public static Popup GetPopup(DependencyObject obj)
        {
            return (Popup)obj.GetValue(PopupProperty);
        }

        /// <summary>
        /// Setter function for <see cref="PopupProperty"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>.</param>
        /// <param name="value">The <see cref="Popup"/>.</param>
        public static void SetPopup(DependencyObject obj, Popup value)
        {
            obj.SetValue(PopupProperty, value);
        }

        /// <summary>
        /// Getter function for <see cref="ControlProperty"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>.</param>
        /// <returns>The <see cref="FrameworkElement"/>.</returns>
        internal static FrameworkElement GetControl(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(ControlProperty);
        }

        /// <summary>
        /// Setter function for <see cref="ControlProperty"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/>.</param>
        /// <param name="value">The <see cref="FrameworkElement"/>.</param>
        internal static void SetControl(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(ControlProperty, value);
        }

        /// <summary>
        /// Handles changes of the pop-up.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/>.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnPopupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FrameworkElement;
            if (control == null)
            {
                return;
            }

            control.MouseEnter -= OnMouseEnter;
            control.MouseEnter += OnMouseEnter;
        }

        /// <summary>
        /// Ensures that the pop-up is visible if the mouse enters the control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/>.</param>
        private static void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control == null)
            {
                return;
            }

            var popup = GetPopup(control);
            if (popup == null)
            {
                return;
            }

            if (popup.IsOpen && !popup.IsMouseCaptureWithin)
            {
                popup.IsOpen = false;
            }

            popup.IsOpen = true;
            var content = popup.Child as FrameworkElement;
            if (content == null)
            {
                popup.IsOpen = false;
                return;
            }

            if (!Mouse.Capture(content, CaptureMode.SubTree))
            {
                popup.IsOpen = false;
                return;
            }

            content.LostMouseCapture += OnLostMouseCapture;
            content.PreviewMouseMove += OnMouseMove;
            content.LayoutUpdated += OnLayoutUpdated;
        }

        /// <summary>
        /// This takes car of hiding the pop-up, if the indicator next to the code moves out of the mouse pointer while typing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        private static void OnLayoutUpdated(object sender, EventArgs e)
        {
            CheckPopupStaysVisible(sender);
        }

        /// <summary>
        /// This takes care of hiding the pop-up, if the mouse move out of the pop-up.
        /// and out of the hit test area of the hint.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/>.</param>
        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            CheckPopupStaysVisible(sender);
        }

        /// <summary>
        /// Hides the pop-up if the prerequisites are not fulfilled anymore.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private static void CheckPopupStaysVisible(object sender)
        {
            var content = sender as FrameworkElement;
            if (content == null)
            {
                return;
            }

            var popup = content.Parent as Popup;
            if (popup == null)
            {
                return;
            }

            var parent = popup.PlacementTarget as FrameworkElement;
            if (IsPointInControl(parent))
            {
                return;
            }

            if (IsPointInControl(content, DefaultTolerance))
            {
                return;
            }

            popup.IsOpen = false;
            Mouse.Capture(null);
        }

        /// <summary>
        /// Hit-tests if the mouse is over the given control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns><see langword="true"/> if the hit test succeeds, otherwise <see langword="false"/>.</returns>
        private static bool IsPointInControl(FrameworkElement control)
        {
            if (control == null)
            {
                return false;
            }

            var point = Mouse.GetPosition(control);
            var hitTestResult = VisualTreeHelper.HitTest(control, point);

            return hitTestResult?.VisualHit != null;
        }

        /// <summary>
        /// Checks if mouse is over the rectangular area plus the given offset.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <param name="tolerance">The additional tolerance area around the control.</param>
        /// <returns><see langword="true"/>, if the mouse is over the area, otherwise <see langword="false"/>.</returns>
        private static bool IsPointInControl(FrameworkElement control, Size tolerance)
        {
            if (control == null)
            {
                return false;
            }

            var point = Mouse.GetPosition(control);
            var rect = new Rect(0, 0, control.ActualWidth, control.ActualHeight);
            rect = Rect.Inflate(rect, tolerance);

            return rect.Contains(point);
        }

        /// <summary>
        /// Hides the pop-up and disconnects all event handlers.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/>.</param>
        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            var content = sender as FrameworkElement;
            if (content == null)
            {
                return;
            }

            var popup = content.Parent as Popup;
            if (popup == null)
            {
                return;
            }

            popup.IsOpen = false;
            content.LostMouseCapture -= OnLostMouseCapture;
            content.PreviewMouseMove -= OnMouseMove;
            content.LayoutUpdated -= OnLayoutUpdated;
        }
    }
}
