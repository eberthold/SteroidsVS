using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Steroids.CodeStructure.Analyzers;
using Steroids.Contracts.UI;

namespace Steroids.CodeStructure.UI
{
    public partial class CodeStructureView : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(CodeStructureView), new PropertyMetadata(false, OnIsOpenChanged));
        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register("IsPinned", typeof(bool), typeof(CodeStructureView), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedNodeContainerProperty = DependencyProperty.Register("SelectedNodeContainer", typeof(ICodeStructureNodeContainer), typeof(CodeStructureView), new PropertyMetadata(null));
        public static readonly DependencyProperty SpaceReservationProperty = DependencyProperty.Register("SpaceReservation", typeof(IAdornmentSpaceReservation), typeof(CodeStructureView), new PropertyMetadata(null));

        private Window _window;

        public CodeStructureView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the space reservation manager.
        /// </summary>
        public IAdornmentSpaceReservation SpaceReservation
        {
            get { return (IAdornmentSpaceReservation)GetValue(SpaceReservationProperty); }
            set { SetValue(SpaceReservationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the last selected node.a
        /// </summary>
        public ICodeStructureNodeContainer SelectedNodeContainer
        {
            get { return (ICodeStructureNodeContainer)GetValue(SelectedNodeContainerProperty); }
            set { SetValue(SelectedNodeContainerProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view is open or not.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view stays open on loosing focus or not.
        /// </summary>
        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        /// <summary>
        /// Handles the prview keyup, to determine if the views needs to be closed or deactivated.
        /// </summary>
        /// <param name="e">Th <see cref="KeyEventArgs"/>.</param>
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            if (e.Key != Key.Escape)
            {
                return;
            }

            if (!IsPinned)
            {
                IsOpen = false;
            }
            else
            {
                DeactivateKeyboardHandling();
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            ActivateKeyboardHandling();
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as CodeStructureView;
            if (view == null)
            {
                return;
            }

            if (view.IsOpen)
            {
                view.ShowCodeStructure();
            }
            else
            {
                view.HideCodeStructure();
            }
        }

        private void ShowCodeStructure()
        {
            SpaceReservation.ActualWidth = Width;
            ActivateKeyboardHandling();
        }

        private void HideCodeStructure()
        {
            SpaceReservation.ActualWidth = 0;
            DeactivateKeyboardHandling();
        }

        private void OnThumbDragged(object sender, DragDeltaEventArgs e)
        {
            Width = Math.Max(ActualWidth - e.HorizontalChange, MinWidth);
            SpaceReservation.ActualWidth = Width;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            _window = Window.GetWindow(this);
            if (_window == null)
            {
                return;
            }

            Mouse.AddPreviewMouseUpHandler(_window, OnPreviewMouseButtonUp);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            if (_window == null)
            {
                return;
            }

            Mouse.RemovePreviewMouseUpHandler(_window, OnPreviewMouseButtonUp);
        }

        private void OnPreviewMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PART_ListBorder.IsMouseOver)
            {
                ActivateKeyboardHandling();
                return;
            }

            if (IsPinned)
            {
                DeactivateKeyboardHandling();
                return;
            }

            IsOpen = false;
        }

        private void OnListItemClicked(object sender, EventArgs e)
        {
            SelectedNodeContainer = sender as ICodeStructureNodeContainer;
        }

        /// <summary>
        /// Activates the view, so it handles keyboard inputs.
        /// </summary>
        private void ActivateKeyboardHandling()
        {
            if (InputManager.Current.IsInMenuMode)
            {
                return;
            }

            var presentationSource = PresentationSource.FromVisual(this);
            if (presentationSource == null)
            {
                return;
            }

            InputManager.Current.PushMenuMode(presentationSource);
            VisualStateManager.GoToState(this, "Activated", false);
            Keyboard.Focus(PART_List);
        }

        /// <summary>
        /// Deactivates the view, so the main editor handles keyboard inputs again.
        /// </summary>
        private void DeactivateKeyboardHandling()
        {
            if (!InputManager.Current.IsInMenuMode)
            {
                return;
            }

            var presentationSource = PresentationSource.FromVisual(this);
            if (presentationSource == null)
            {
                return;
            }

            InputManager.Current.PopMenuMode(presentationSource);
            VisualStateManager.GoToState(this, "Deactivated", false);
        }
    }
}
