using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Steroids.SharedUI.Behaviors
{
    public static class MouseOverPopupBehavior
    {
        public static readonly DependencyProperty PopupProperty =
            DependencyProperty.RegisterAttached("Popup", typeof(Popup), typeof(MouseOverPopupBehavior), new PropertyMetadata(null, OnPopupChanged));

        internal static readonly DependencyProperty ControlProperty =
            DependencyProperty.RegisterAttached("Control", typeof(FrameworkElement), typeof(MouseOverPopupBehavior), new PropertyMetadata(null));

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
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
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

            var position = e.GetPosition(content);
            var rect = new Rect(0, 0, content.ActualWidth, content.ActualHeight);
            rect = Rect.Inflate(rect, 20, 10);
            if (rect.Contains(position))
            {
                return;
            }

            popup.IsOpen = false;
            Mouse.Capture(null);
        }

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
        }
    }
}
