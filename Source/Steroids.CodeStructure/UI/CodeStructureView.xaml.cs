using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Steroids.CodeStructure.Analyzers;
using Steroids.Contracts.UI;

namespace Steroids.CodeStructure.UI
{
    public partial class CodeStructureView : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(CodeStructureView), new PropertyMetadata(false));
        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register("IsPinned", typeof(bool), typeof(CodeStructureView), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedNodeContainerProperty = DependencyProperty.Register("SelectedNodeContainer", typeof(ICodeStructureNodeContainer), typeof(CodeStructureView), new PropertyMetadata(null));
        public static readonly DependencyProperty SpaceReservationProperty = DependencyProperty.Register("SpaceReservation", typeof(IAdornmentSpaceReservation), typeof(CodeStructureView), new PropertyMetadata(null));

        private static readonly IReadOnlyCollection<Key> KeysToHandle = new List<Key>
        {
            Key.Down, Key.Up, Key.Space, Key.Enter, Key.Escape
        };

        private Window _window;
        private bool _skipSelectionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureView"/> class.
        /// </summary>
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
        /// Gets or sets the last selected node.a.
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
        /// Handles the preview key-up, to determine if the views needs to be closed or deactivated.
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

        private void OnThumbDragged(object sender, DragDeltaEventArgs e)
        {
            Width = Math.Max(ActualWidth - e.HorizontalChange, MinWidth);
            SpaceReservation.ActualWidth = Width;
        }

        /// <summary>
        /// Attaches all event handlers we need to have a nice behavior in this view.
        /// </summary>
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

            Mouse.AddPreviewMouseDownHandler(_window, OnPreviewMouseButtonDown);
        }

        /// <summary>
        /// Cleaning up when this view is no longer needed.
        /// </summary>
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

            Mouse.RemovePreviewMouseDownHandler(_window, OnPreviewMouseButtonDown);
        }

        /// <summary>
        /// Handles mouse clicks which occur in the hit-test area of this view.
        /// </summary>
        private void OnPreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PART_Toolbar.IsMouseOver)
            {
                ActivateKeyboardHandling();
                return;
            }

            if (PART_ListBorder.IsMouseOver)
            {
                var wasItemClick = false;
                var elementToCheck = e.OriginalSource as DependencyObject;
                while ((elementToCheck = VisualTreeHelper.GetParent(elementToCheck)) != null)
                {
                    if (!(elementToCheck is ListViewItem))
                    {
                        continue;
                    }

                    wasItemClick = true;
                    break;
                }

                ActivateKeyboardHandling();
                e.Handled = !wasItemClick;
                return;
            }

            if (!IsPinned)
            {
                IsOpen = false;
                return;
            }

            if (IsKeyboardFocusWithin)
            {
                DeactivateKeyboardHandling();
            }
        }

        /// <summary>
        /// Activates the view, so it handles keyboard inputs.
        /// </summary>
        private void ActivateKeyboardHandling()
        {
            if (InputManager.Current.IsInMenuMode)
            {
                Keyboard.Focus(PART_FilterText);
                return;
            }

            var presentationSource = PresentationSource.FromVisual(this);
            if (presentationSource == null)
            {
                return;
            }

            InputManager.Current.PushMenuMode(presentationSource);
            VisualStateManager.GoToState(this, "Activated", false);
            Keyboard.Focus(PART_FilterText);
        }

        /// <summary>
        /// Deactivates the view, so the main editor handles keyboard inputs again.
        /// </summary>
        private void DeactivateKeyboardHandling()
        {
            if (!InputManager.Current.IsInMenuMode)
            {
                Keyboard.ClearFocus();
                return;
            }

            var presentationSource = PresentationSource.FromVisual(this);
            if (presentationSource == null)
            {
                return;
            }

            InputManager.Current.PopMenuMode(presentationSource);
            VisualStateManager.GoToState(this, "Deactivated", false);
            Keyboard.ClearFocus();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PART_FilterText.IsVisible)
            {
                ShowCodeStructure();
            }
            else
            {
                HideCodeStructure();
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

        /// <summary>
        /// Handles the most important keys of the <see cref="PART_FilterText"/>.
        /// Those keys are handled, so we can navigate and select in the <see cref="PART_List"/>,
        /// without loosing keyboard focus in the list box.
        /// </summary>
        private void OnTextKeyDown(object sender, KeyEventArgs e)
        {
            _skipSelectionChanged = true;
            if (!KeysToHandle.Contains(e.Key))
            {
                return;
            }

            var collectionView = PART_List.ItemsSource as ICollectionView ?? CollectionViewSource.GetDefaultView(PART_List.ItemsSource);

            switch (e.Key)
            {
                case Key.Up:
                    collectionView.MoveCurrentToPrevious();
                    break;

                case Key.Down:
                    collectionView.MoveCurrentToNext();
                    break;

                case Key.Enter:
                case Key.Space:
                    _skipSelectionChanged = false;
                    SelectedNodeContainer = collectionView.CurrentItem as ICodeStructureNodeContainer;
                    break;

                case Key.Escape:
                    PART_FilterText.Text = string.Empty;
                    break;
            }

            e.Handled = true;
        }

        private void OnListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_skipSelectionChanged)
            {
                _skipSelectionChanged = false;
                return;
            }

            SelectedNodeContainer = PART_List.SelectedItem as ICodeStructureNodeContainer;
            ActivateKeyboardHandling();
            e.Handled = true;
        }
    }
}
