using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Steroids.Contracts.UI;

namespace Steroids.CodeStructure.UI
{
    public partial class CodeStructureView : UserControl
    {
        private readonly IAdornmentSpaceReservation _spaceReservation;
        private readonly CodeStructureViewModel _viewModel;

        private Window _window;

        public CodeStructureView(CodeStructureViewModel viewModel, IAdornmentSpaceReservation spaceReservation)
        {
            InitializeComponent();

            _spaceReservation = spaceReservation ?? throw new ArgumentNullException(nameof(spaceReservation));
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;
        }

        private void OnThumbDragged(object sender, DragDeltaEventArgs e)
        {
            Width = Math.Max(ActualWidth - e.HorizontalChange, MinWidth);
            _spaceReservation.ActualWidth = Width;
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
                return;
            }

            if (_viewModel.IsPinned)
            {
                return;
            }

            HideCodeStructure();
        }

        private void OnListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = sender as ListView;
            if (list?.SelectedItem == null)
            {
                return;
            }

            list.SelectedItem = null;
        }

        private void OnIndicatorChecked(object sender, RoutedEventArgs e)
        {
            ShowCodeStructure();
        }

        private void ShowCodeStructure()
        {
            _viewModel.IsListVisible = true;
            _spaceReservation.ActualWidth = Width;
        }

        private void HideCodeStructure()
        {
            _viewModel.IsListVisible = false;
            _spaceReservation.ActualWidth = 0;
        }
    }
}
