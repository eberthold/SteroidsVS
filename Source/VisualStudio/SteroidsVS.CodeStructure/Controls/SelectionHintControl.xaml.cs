﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SteroidsVS.CodeStructure.Controls
{
    /// <summary>
    /// Interaction logic for SelectionHintControl.xaml.
    /// </summary>
    public partial class SelectionHintControl : UserControl
    {
        public SelectionHintControl()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var storyboard = Resources["SlideAnimation"] as Storyboard;
            storyboard.Begin();
        }
    }
}
