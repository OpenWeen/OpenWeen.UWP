using System;
using System.Collections.Generic;
using OpenWeen.UWP.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OpenWeen.UWP.Common.Controls
{
    public sealed partial class PivotHeader : UserControl
    {
        public event EventHandler BackToTop;

        public event EventHandler Refresh;

        public List<HeaderModel> ItemsSource { get; set; }

        public PivotHeader()
        {
            this.InitializeComponent();
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(PivotHeader), new PropertyMetadata(-1, OnSelectedIndexChanged));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PivotHeader).ChangeSelectedIndex((int)e.NewValue, (int)e.OldValue);
        }

        public void ChangeSelectedIndex(int newValue, int oldValue)
        {
            try
            {
                ItemsSource[newValue].IsActive = true;
                ItemsSource[oldValue].IsActive = false;
                if (ItemsSource[newValue].UnreadCount > 0)
                {
                    ItemsSource[newValue].UnreadCount = 0;
                    Refresh?.Invoke(this, new EventArgs());
                }
            }
            catch { }
        }

        private void ItemsControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var index = ItemsSource.FindIndex(item => item == (e.OriginalSource as FrameworkElement).DataContext as HeaderModel);
            if (SelectedIndex == index)
            {
                BackToTop?.Invoke(this, new EventArgs());
                try
                {
                    if (ItemsSource[index].UnreadCount > 0)
                    {
                        ItemsSource[index].UnreadCount = 0;
                        Refresh?.Invoke(this, new EventArgs());
                    }
                }
                catch { }
            }
            else
            {
                SelectedIndex = index;
            }
        }

        private void ItemsControl_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var index = ItemsSource.FindIndex(item => item == (e.OriginalSource as FrameworkElement).DataContext as HeaderModel);
            try
            {
                if (ItemsSource[index].UnreadCount > 0)
                {
                    ItemsSource[index].UnreadCount = 0;
                }
                Refresh?.Invoke(this, new EventArgs());
            }
            catch { }
        }
    }
}