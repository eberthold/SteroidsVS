﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.Contracts.UI;

namespace Steroids.CodeQuality.UI
{
    /// <summary>
    /// Special panel for placing Items relative to the <see cref="IWpfTextView"/>.
    /// </summary>
    public class DiagnosticInfoLinePanel : Panel
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items",
            typeof(ObservableCollection<DiagnosticInfoLine>),
            typeof(DiagnosticInfoLinePanel),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                OnItemsChanged));

        public static readonly DependencyProperty AdornmentSpaceReservationProperty = DependencyProperty.Register(
            "AdornmentSpaceReservation",
            typeof(IAdornmentSpaceReservation),
            typeof(DiagnosticInfoLinePanel),
            new PropertyMetadata(null));

        public static readonly DependencyProperty TextViewProperty = DependencyProperty.Register(
            "TextView",
            typeof(IWpfTextView),
            typeof(DiagnosticInfoLinePanel),
            new PropertyMetadata(null));

        private readonly List<ContentControl> _containerPool = new List<ContentControl>();
        private readonly Dictionary<DiagnosticInfoLine, Rect> _placementMap = new Dictionary<DiagnosticInfoLine, Rect>();

        /// <summary>
        /// The space reservation needed for placement calculations.
        /// </summary>
        public IAdornmentSpaceReservation AdornmentSpaceReservation
        {
            get { return (IAdornmentSpaceReservation)GetValue(AdornmentSpaceReservationProperty); }
            set { SetValue(AdornmentSpaceReservationProperty, value); }
        }

        /// <summary>
        /// The items to display.
        /// </summary>
        public ObservableCollection<DiagnosticInfoLine> Items
        {
            get { return (ObservableCollection<DiagnosticInfoLine>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// The document on which the <see cref="Items"/> are placed.
        /// </summary>
        public IWpfTextView TextView
        {
            get { return (IWpfTextView)GetValue(TextViewProperty); }
            set { SetValue(TextViewProperty, value); }
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var item in Items.ToList())
            {
                var placement = DiagnosticInfoPlacementCalculator.CalculatePlacementRect(TextView, item, AdornmentSpaceReservation);
                if (_placementMap.ContainsKey(item))
                {
                    _placementMap[item] = placement;
                }
                else
                {
                    _placementMap.Add(item, placement);
                }
            }

            return availableSize;
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // make unused items available again in pool
            var childs = Children.Cast<ContentControl>();
            foreach (var child in childs.Where(x => _placementMap[x.Content as DiagnosticInfoLine] == Rect.Empty))
            {
                Children.Remove(child);
                child.Content = null;
            }

            // arrange all items with valid placement info
            foreach (var item in _placementMap.Where(x => x.Value != Rect.Empty).ToList())
            {
                var container = _containerPool.Find(x => x.Content == item.Key)
                    ?? _containerPool.Find(x => x.Content == null)
                    ?? new ContentControl();

                if (!_containerPool.Contains(container))
                {
                    _containerPool.Add(container);
                }

                if (!Children.Contains(container))
                {
                    Children.Add(container);
                }

                container.Arrange(item.Value);
            }

            return finalSize;
        }

        /// <summary>
        /// Handles changes of the <see cref="Items"/> collection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var panel = sender as DiagnosticInfoLinePanel;
            if (panel == null)
            {
                return;
            }

            if (args.OldValue is ObservableCollection<DiagnosticInfoLine> oldItems)
            {
                oldItems.CollectionChanged -= panel.OnItemsCollectionChanged;
            }

            if (args.NewValue is ObservableCollection<DiagnosticInfoLine> newItems)
            {
                newItems.CollectionChanged += panel.OnItemsCollectionChanged;
            }
        }

        /// <summary>
        /// Triggered when the <see cref="Items"/> collection changed.
        /// Triggers a new layout of the panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/>.</param>
        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateMeasure();
        }
    }
}
