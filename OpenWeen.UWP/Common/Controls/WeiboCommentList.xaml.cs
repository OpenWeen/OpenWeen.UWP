using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Comment;
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
    public sealed partial class WeiboCommentList : ItemsControl
    {
        public event EventHandler LoadMore;
        public event EventHandler<WeiboActionEventArgs> Comment;
        public event EventHandler<WeiboItemClickEventArgs> ItemClick;
        public event EventHandler<WeiboUserClickEventArgs> UserClick;

        public WeiboCommentList()
        {
            this.InitializeComponent();
        }

        private void Comment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            Comment?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as CommentModel));
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            var verticalOffsetValue = scrollViewer.VerticalOffset;
            var maxVerticalOffsetValue = scrollViewer.ExtentHeight - scrollViewer.ViewportHeight;
            if (maxVerticalOffsetValue < 0 || verticalOffsetValue == maxVerticalOffsetValue)
            {
                LoadMore?.Invoke(this, new EventArgs());
            }
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
