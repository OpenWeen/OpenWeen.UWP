﻿using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Entities;
using OpenWeen.UWP.Common.Helpers;
using OpenWeen.UWP.Model;
using OpenWeen.UWP.View;
using OpenWeen.UWP.ViewModel;
using OpenWeen.UWP.ViewModel.MainPage;
using OpenWeen.UWP.ViewModel.SearchPage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace OpenWeen.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel MainVM { get; } = new MainPageViewModel();
        public WeiboActionModel ActionModel { get; } = WeiboActionModel.Instance;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }
        public void BackTop()
        {
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(pivot.SelectedItem as PivotItem);
            scrollViewer.ChangeView(0, 0, 1f, false);
        }

        private void Ellipse_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ActionModel.UserClick(null, new Common.Controls.Events.WeiboUserClickEventArgs(StaticResource.Uid));
        }
        public void PostWeibo()
        {
            Frame.Navigate(typeof(PostWeiboPage), new PostWeibo());
        }
        public void Setting()
        {
            Frame.Navigate(typeof(SettingPage));
        }
        public void Search()
        {
            Frame.Navigate(typeof(SearchPage), new SearchPageViewModel());
        }
        public void ShowGroup(object sender, RoutedEventArgs e)
        {
            var menu = Resources["GroupFlout"] as Flyout;
            menu.Placement = (Window.Current.Content as Frame).ActualWidth > 720 ? Windows.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Right : Windows.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Top;
            menu.ShowAt(e.OriginalSource as FrameworkElement);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var menu = Resources["GroupFlout"] as Flyout;
            menu.Hide();
        }
    }
}