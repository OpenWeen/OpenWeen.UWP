using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using OpenWeen.Core.Model.User;
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
    public sealed partial class UserList : ItemsControl
    {
        public event EventHandler LoadMore;
        public event EventHandler<Events.WeiboUserClickEventArgs> UserClick;
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(UserList), new PropertyMetadata(false));


        public UserList()
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

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UserClick?.Invoke(this, new Events.WeiboUserClickEventArgs(((sender as FrameworkElement).DataContext as UserModel).ID));
        }
    }
}
