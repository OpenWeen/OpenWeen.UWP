using System;
using System.Collections;
using OpenWeen.Core.Model;
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
    public sealed partial class WeiboListView : ItemsControl
    {
        public event EventHandler<ScrollViewerViewChangedEventArgs> ViewChanged;

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(WeiboListView), new PropertyMetadata(null));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(WeiboListView), new PropertyMetadata(false));

        public WeiboListView()
        {
            this.InitializeComponent();
        }
        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ViewChanged?.Invoke(sender, e);
        }
    }
}