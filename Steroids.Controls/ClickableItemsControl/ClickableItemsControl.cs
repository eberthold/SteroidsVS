using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Steroids.Controls
{
    public class ClickableItemsControl : ItemsControl
    {
        static ClickableItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClickableItemsControl), new FrameworkPropertyMetadata(typeof(ClickableItemsControl)));
        }

        public event EventHandler ItemClicked;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var container = ItemContainerGenerator.ContainerFromItem(item) as ClickableItemContainer;
            if (container == null)
            {
                return;
            }

            container.Content = item;
            container.ContentTemplateSelector = ItemTemplateSelector;
            container.PreviewMouseLeftButtonUp += Container_PreviewMouseLeftButtonUp;
            container.PreviewKeyUp += Container_PreviewKeyUp;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            var container = ItemContainerGenerator.ContainerFromItem(item) as ClickableItemContainer;
            if (container == null)
            {
                return;
            }

            container.Content = null;
            container.PreviewMouseLeftButtonUp -= Container_PreviewMouseLeftButtonUp;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ClickableItemContainer;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ClickableItemContainer();
        }

        private void Container_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return && e.Key != Key.Space)
            {
                return;
            }

            var container = sender as ClickableItemContainer;
            if (container == null)
            {
                return;
            }

            ItemClicked?.Invoke(container.DataContext, EventArgs.Empty);
        }

        private void Container_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var container = sender as ClickableItemContainer;
            if (container == null)
            {
                return;
            }

            ItemClicked?.Invoke(container.DataContext, EventArgs.Empty);
        }
    }
}
