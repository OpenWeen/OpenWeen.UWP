using OpenWeen.Core.Model.Status;
using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common.Controls.Events;
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
    public sealed partial class WeiboList : ItemsControl
    {
        public event EventHandler<WeiboUserClickEventArgs> UserClick;
        public event EventHandler<WeiboItemClickEventArgs> ItemClick;
        public event EventHandler<WeiboPictureClickEventArgs> PictureClick;
        public event EventHandler<WeiboTopicClickEventArgs> TopicClick;
        public event EventHandler<WeiboActionEventArgs> Repost;
        public event EventHandler<WeiboActionEventArgs> Comment;
        public event EventHandler<WeiboActionEventArgs> Favor;

        public event EventHandler LoadMore;

        public WeiboList()
        {
            this.InitializeComponent();
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

        private void WeiboImageList_PictureClick(object sender, WeiboPictureClickEventArgs e)
        {
            e.DataContext = (sender as WeiboImageList).DataContext as MessageModel;
            PictureClick?.Invoke(sender, e);
        }

        private void Reshare_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            Repost?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as MessageModel));
        }

        private void Comment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            Comment?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as MessageModel));
        }

        private void Favor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            Favor?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as MessageModel));
        }
        

        private void WeiboTextBlock_UserClick(object sender, WeiboUserClickEventArgs e)
        {
            UserClick?.Invoke(this, e);
        }

        private void WeiboTextBlock_TopicClick(object sender, WeiboTopicClickEventArgs e)
        {
            TopicClick?.Invoke(this, e);
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
