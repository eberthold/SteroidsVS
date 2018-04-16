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
    // TODO: This file is dirty and mixing viewmodel calls with ui stuff, clean up here
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

        public bool IsOpen
        {
            get { return _viewModel.IsListVisible; }
        }

        public void ShowCodeStructure()
        {
            _viewModel.IsListVisible = true;
            _spaceReservation.ActualWidth = Width;

            Activate();
        }

        public void HideCodeStructure()
        {
            _viewModel.IsListVisible = false;
            _spaceReservation.ActualWidth = 0;

            Deactivate();
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            if (e.Key != Key.Escape)
            {
                return;
            }

            Deactivate();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            Activate();
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
                Activate();
                return;
            }

            if (_viewModel.IsPinned)
            {
                Deactivate();
                return;
            }

            HideCodeStructure();
        }

        private void OnIndicatorChecked(object sender, RoutedEventArgs e)
        {
            ShowCodeStructure();
        }

        private void OnListItemClicked(object sender, EventArgs e)
        {
            _viewModel.SelectedNode = sender as ICodeStructureNodeContainer;
        }

        private void Activate()
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
        }

        private void Deactivate()
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
