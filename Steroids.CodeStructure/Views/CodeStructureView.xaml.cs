namespace Steroids.CodeStructure.Views
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Steroids.CodeStructure.ViewModels;

    public partial class CodeStructureView : UserControl
    {
        private Window _window;

        public CodeStructureView()
        {
            InitializeComponent();
        }

        private void OnThumbDragged(object sender, DragDeltaEventArgs e)
        {
            Width = Math.Max(ActualWidth - e.HorizontalChange, MinWidth);
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

            var vm = DataContext as CodeStructureViewModel;
            if (vm == null || vm.IsPinned)
            {
                return;
            }

            vm.IsListVisible = false;
        }
    }
}
