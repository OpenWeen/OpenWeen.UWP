using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OpenWeen.UWP.Common.Controls
{
    public sealed partial class MessageList : ItemsControl
    {
        public event EventHandler LoadMore;
        private ScrollViewer _scrollViewer;
        private bool _isLoadMore;

        public MessageList()
        {
            this.InitializeComponent();
            Loaded += MessageList_Loaded;
        }

        private void MessageList_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = Helpers.MoreVisualTreeHelper.GetObject<ScrollViewer>(this);
            _scrollViewer.RegisterPropertyChangedCallback(ScrollViewer.ScrollableHeightProperty, OnContentChanged);
        }

        private async void OnContentChanged(DependencyObject sender, DependencyProperty dp)
        {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (_isLoadMore)
                {
                    _isLoadMore = false;
                }
                else
                {
                    _scrollViewer.ChangeView(0, _scrollViewer.ScrollableHeight, 1);
                }
            });
        }
        

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_scrollViewer.VerticalOffset == 0)
            {
                LoadMore?.Invoke(this, new EventArgs());
                _isLoadMore = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadMore?.Invoke(this, new EventArgs());
            _isLoadMore = true;
        }
    }
}
