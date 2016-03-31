using System;
using OpenWeen.Core.Model.Status;
using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common.Controls.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OpenWeen.UWP.Common.Controls
{
    public sealed partial class WeiboDetailRepostList : ItemsControl
    {
        public event EventHandler<WeiboUserClickEventArgs> UserClick;

        public event EventHandler<WeiboItemClickEventArgs> ItemClick;

        public event EventHandler<WeiboTopicClickEventArgs> TopicClick;

        public event EventHandler LoadMore;

        public WeiboDetailRepostList()
        {
            this.InitializeComponent();
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

        private void WeiboTextBlock_UserClick(object sender, WeiboUserClickEventArgs e)
        {
            UserClick?.Invoke(this, e);
        }

        private void WeiboTextBlock_TopicClick(object sender, WeiboTopicClickEventArgs e)
        {
            TopicClick?.Invoke(this, e);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var context = (e.OriginalSource as FrameworkElement).DataContext;
            if (context is UserModel)
            {
                e.Handled = true;
                UserClick?.Invoke(this, new WeiboUserClickEventArgs((context as UserModel).ID));
            }
            else if (context is MessageModel)
            {
                e.Handled = true;
                ItemClick?.Invoke(this, new WeiboItemClickEventArgs(context as MessageModel));
            }
        }
    }
}