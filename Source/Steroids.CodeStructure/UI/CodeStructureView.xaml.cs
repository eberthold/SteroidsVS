﻿using System;
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
using Steroids.Contracts.UI;

namespace Steroids.CodeStructure.UI
{
    public partial class CodeStructureView : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(CodeStructureView), new PropertyMetadata(false, OnIsOpenChanged));

        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register("IsPinned", typeof(bool), typeof(CodeStructureView), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedNodeContainerProperty = DependencyProperty.Register("SelectedNodeContainer", typeof(ICodeStructureNodeContainer), typeof(CodeStructureView), new PropertyMetadata(null));
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

            base.OnGotKeyboardFocus(e);
            ActivateKeyboardHandling();
            e.Handled = true;
        }

        /// <summary>
        /// Handles the mouse down event, to hide the view, select an item or focus the filter text.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/>.</param>
        protected void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseOver)
            {
                HideCodeStructure();
                return;
            }

            if (PART_List.IsMouseOver)
            {
                _skipFocusHandler = true;
                return;
            }

            ActivateKeyboardHandling();
            e.Handled = true;
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
            Keyboard.Focus(this);
        }

        /// <summary>
        /// Deactivates the view, so the main editor handles keyboard inputs again.
        /// </summary>
        private void DeactivateKeyboardHandling()
        {
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

            IsOpen = false;
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
                    SelectedNodeContainer = collectionView.CurrentItem as ICodeStructureNodeContainer;
                    e.Handled = true;
                    break;

                case Key.Escape:
                    PART_FilterText.Text = string.Empty;
                    HideCodeStructure();
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

            SelectedNodeContainer = PART_List.SelectedItem as ICodeStructureNodeContainer;
            ActivateKeyboardHandling();
            e.Handled = true;
        }
    }
}
