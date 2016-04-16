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
    public sealed partial class MessageUserList : ItemsControl
    {
        public event EventHandler LoadMore;
        public event EventHandler<Events.WeiboMessageItemClickEventArgs> ItemClick;

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(MessageUserList), new PropertyMetadata(false));



        public MessageUserList()
        {
            this.InitializeComponent();
        }
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            ItemClick?.Invoke(this, new Events.WeiboMessageItemClickEventArgs((e.OriginalSource as FrameworkElement).DataContext as Core.Model.DirectMessage.DirectMessageUserModel));
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            var verticalOffsetValue = scrollViewer.VerticalOffset;
            var maxVerticalOffsetValue = scrollViewer.ExtentHeight - scrollViewer.ViewportHeight;
            if (maxVerticalOffsetValue < 0 || verticalOffsetValue == maxVerticalOffsetValue)
            {
                LoadMore?.Invoke(this, new EventArgs());
            }
        }
    }
}
