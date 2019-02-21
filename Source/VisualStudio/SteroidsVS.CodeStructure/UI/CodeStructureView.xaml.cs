using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.CodeStructure.Analyzers;
using Steroids.Core.UI;

namespace SteroidsVS.CodeStructure.UI
{
    public partial class CodeStructureView : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(CodeStructureView), new PropertyMetadata(false, OnIsOpenChanged));

        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register("IsPinned", typeof(bool), typeof(CodeStructureView), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedNodeContainerProperty = DependencyProperty.Register("SelectedNodeContainer", typeof(ICodeStructureItem), typeof(CodeStructureView), new PropertyMetadata(null));
        public static readonly DependencyProperty SpaceReservationProperty = DependencyProperty.Register("SpaceReservation", typeof(IAdornmentSpaceReservation), typeof(CodeStructureView), new PropertyMetadata(null));

        private static readonly IReadOnlyCollection<Key> KeysToHandle = new List<Key>
        {
            Key.Down, Key.Up, Key.Space, Key.Enter, Key.Escape
        };

        private bool _skipSelectionChanged;
        private bool _skipFocusHandler;
        private DependencyObject _textView;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureView"/> class.
        /// </summary>
        public CodeStructureView()
        {
            InitializeComponent();
            CommandRouting.SetInterceptsCommandRouting(this, true);

            Loaded += OnLoaded;
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
        public ICodeStructureItem SelectedNodeContainer
        {
            get { return (ICodeStructureItem)GetValue(SelectedNodeContainerProperty); }
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
        /// Promotes keyboard foucs to the <see cref="PART_FilterText"/>.
        /// </summary>
        /// <param name="e">The <see cref="KeyboardFocusChangedEventArgs"/>.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (e.Source == this && _skipFocusHandler)
            {
                _skipFocusHandler = false;
                return;
            }

            PART_FilterText.Focus();
            e.Handled = true;
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Handles the mouse down event, to hide the view, select an item or focus the filter text<see cref="PART_FilterText"/>.
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/>.</param>
        protected void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseOver)
            {
                IsOpen = IsPinned;
                return;
            }

            if ((PART_List.IsMouseOver && !IsMouseOverSpareListViewSpace()) || PART_Toolbar.IsMouseOver)
            {
                return;
            }

            PART_FilterText.Focus();
            e.Handled = true;
        }

        protected void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PART_ClickCatcher.IsMouseDirectlyOver)
            {
                e.Handled = true;
            }

            if (IsMouseOverSpareListViewSpace())
            {
                e.Handled = true;
            }

            if (PART_List.IsMouseOver || PART_Toolbar.IsMouseOver)
            {
                _skipFocusHandler = true;
                return;
            }
        }

        /// <summary>
        /// Changes the visual state based on the <see cref="IsKeyboardFocusWithin"/>.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/>.</param>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsKeyboardFocusWithin)
            {
                VisualStateManager.GoToState(this, "Activated", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Deactivated", false);
            }
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var codeStructure = d as CodeStructureView;
            if (codeStructure is null)
            {
                return;
            }

            if (codeStructure.IsOpen)
            {
                codeStructure.ShowCodeStructure();
            }
            else
            {
                codeStructure.HideCodeStructure();
            }
        }

        /// <summary>
        /// Checks if the mouse is over the <see cref="PART_List"/> but not over one of it's items.
        /// </summary>
        /// <returns><see langword="true"/>, if the mouse is over the "free" area of the <see cref="PART_List"/>.</returns>
        private bool IsMouseOverSpareListViewSpace()
        {
            return PART_List.IsMouseOver && Mouse.DirectlyOver is ScrollViewer;
        }

        /// <summary>
        /// Gets the surrounding textview and adds the mouse handler.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DependencyObject current = this;
            while (!(current is IWpfTextView) && current != null)
            {
                current = VisualTreeHelper.GetParent(current);
            }

            _textView = (current as IWpfTextView)?.VisualElement ?? current;

            Mouse.AddMouseUpHandler(_textView, OnMouseUp);
            Mouse.AddPreviewMouseDownHandler(_textView, OnPreviewMouseDown);
        }

        /// <summary>
        /// Resizes the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DragDeltaEventArgs"/>.</param>
        private void OnThumbDragged(object sender, DragDeltaEventArgs e)
        {
            Width = Math.Max(ActualWidth - e.HorizontalChange, MinWidth);
            SpaceReservation.ActualWidth = Width;
        }

        /// <summary>
        /// Activates the view, so it handles keyboard inputs.
        /// </summary>
        private void ActivateKeyboardHandling()
        {
            if (PART_FilterText.Focusable && PART_FilterText.IsVisible)
            {
                Keyboard.Focus(PART_FilterText);
                FocusManager.SetFocusedElement(this, PART_FilterText);
            }
        }

        /// <summary>
        /// Deactivates the view, so the main editor handles keyboard inputs again.
        /// </summary>
        private void DeactivateKeyboardHandling()
        {
            Keyboard.ClearFocus();
            Keyboard.Focus(_textView as IInputElement);
        }

        private void ShowCodeStructure()
        {
            SpaceReservation.ActualWidth = Width;
            ActivateKeyboardHandling();
        }

        private void HideCodeStructure()
        {
            if (IsPinned)
            {
                return;
            }

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
                    e.Handled = true;
                    break;

                case Key.Down:
                    collectionView.MoveCurrentToNext();
                    e.Handled = true;
                    break;

                case Key.Enter:
                case Key.Space:
                    _skipSelectionChanged = false;
                    SelectedNodeContainer = collectionView.CurrentItem as ICodeStructureItem;
                    PART_List.SelectedItem = null;
                    e.Handled = true;
                    break;

                case Key.Escape:
                    PART_FilterText.Text = string.Empty;
                    IsOpen = IsPinned;
                    DeactivateKeyboardHandling();
                    break;
            }
        }

        private void OnListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_skipSelectionChanged)
            {
                _skipSelectionChanged = false;
                return;
            }

            if (PART_List.SelectedItem is null)
            {
                return;
            }

            SelectedNodeContainer = PART_List.SelectedItem as ICodeStructureItem;
            PART_List.SelectedItem = null;
            _skipFocusHandler = false;
            PART_FilterText.Focus();
            e.Handled = true;
        }

        /// <summary>
        /// The <see cref="PART_FilterText"/> only can gain focus if it is visible, so we need to handle this event.
        /// </summary>
        private void OnFilterTextIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!PART_FilterText.IsVisible)
            {
                return;
            }

            ActivateKeyboardHandling();
        }
    }
}
