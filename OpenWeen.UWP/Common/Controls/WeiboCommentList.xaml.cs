﻿using System;
using OpenWeen.Core.Model.Comment;
using OpenWeen.Core.Model.Status;
using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common.Controls.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OpenWeen.UWP.Common.Controls
{
    public sealed partial class WeiboCommentList : ItemsControl
    {
        public event EventHandler LoadMore;

        public event EventHandler<WeiboActionEventArgs> Comment;

        public event EventHandler<WeiboItemClickEventArgs> ItemClick;

        public event EventHandler<WeiboUserClickEventArgs> UserClick;
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(WeiboCommentList), new PropertyMetadata(false));



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
            e.Handled = true;
            if (context is UserModel)
            {
                UserClick?.Invoke(this, new WeiboUserClickEventArgs((context as UserModel).ID));
            }
            else if (context is MessageModel)
            {
                ItemClick?.Invoke(this, new WeiboItemClickEventArgs(context as MessageModel));
            }
            else
            {
                Comment?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as CommentModel));
            }
        }
    }
}