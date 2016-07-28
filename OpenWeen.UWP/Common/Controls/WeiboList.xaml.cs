using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Comment;
using OpenWeen.Core.Model.Status;
using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common.Controls.Events;
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
    public sealed partial class WeiboList : UserControl
    {
        public event EventHandler<WeiboUserClickEventArgs> UserClick;

        public event EventHandler<WeiboItemClickEventArgs> ItemClick;

        public event EventHandler<WeiboPictureClickEventArgs> PictureClick;

        public event EventHandler<WeiboTopicClickEventArgs> TopicClick;

        public event EventHandler<WeiboActionEventArgs> Repost;

        public event EventHandler<WeiboActionEventArgs> Comment;

        public event EventHandler<WeiboActionEventArgs> Favor;

        public event EventHandler<WeiboActionEventArgs> Delete;

        public event EventHandler<WeiboActionEventArgs> Like;

        public event EventHandler LoadMore;

        public event EventHandler<ScrollViewerViewChangedEventArgs> ViewChanged;


        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(WeiboList), new PropertyMetadata(null));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(WeiboList), new PropertyMetadata(false));


        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(WeiboList), new PropertyMetadata(null));



        public bool IsRepostList
        {
            get { return WeiboListTemplateSelector.IsRepostList; }
            set { WeiboListTemplateSelector.IsRepostList = value; }
        }


        public WeiboList()
        {
            this.InitializeComponent();
            this.DataContext = this;
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
            else if (context is CommentModel)
            {
                Comment?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as CommentModel));
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
            Repost?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as BaseModel));
        }

        private void Comment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            Comment?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as BaseModel));
        }

        private void Favor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            Favor?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as BaseModel));
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
            ViewChanged?.Invoke(sender, e);
            var verticalOffsetValue = scrollViewer.VerticalOffset;
            var maxVerticalOffsetValue = scrollViewer.ExtentHeight - scrollViewer.ViewportHeight;
            if (maxVerticalOffsetValue < 0 || verticalOffsetValue == maxVerticalOffsetValue)
            {
                LoadMore?.Invoke(this, new EventArgs());
            }
        }

        private void Delete_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            Delete?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as BaseModel));
            (ItemsSource as IList).Remove((e.OriginalSource as FrameworkElement).DataContext);
        }

        private void Like_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            Like?.Invoke(this, new WeiboActionEventArgs((e.OriginalSource as FrameworkElement).DataContext as BaseModel));
        }
    }
}
