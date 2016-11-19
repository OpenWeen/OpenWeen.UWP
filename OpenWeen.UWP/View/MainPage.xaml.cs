using System.ComponentModel;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Entities;
using OpenWeen.UWP.Common.Helpers;
using OpenWeen.UWP.Model;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.View;
using OpenWeen.UWP.ViewModel;
using OpenWeen.UWP.ViewModel.MainPage;
using OpenWeen.UWP.ViewModel.SearchPage;
using Windows.UI.Core;
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
        public MainPageViewModel ViewModel { get; } = MainPageViewModel.Instance;
        public WeiboActionModel ActionModel { get; } = WeiboActionModel.Instance;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                ViewModel.Initialization();
                if (Settings.IsMergeMentionAndComment)
                {
                    pivot.Items.RemoveAt(2);
                    pivot.Items.RemoveAt(2);
                    pivot.Items.RemoveAt(2);
                }
                else
                {
                    pivot.Items.RemoveAt(1);
                }
                Frame.BackStack.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }
        public void BackTop()
        {
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(pivot.SelectedItem as PivotItem);
            scrollViewer?.ChangeView(0, 0, 1f, false);
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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var menu = Resources["GroupFlout"] as Flyout;
            menu.Hide();
        }

        private void pivot_Loaded(object sender, RoutedEventArgs e)
        {
            MoreVisualTreeHelper.GetObjectByName<ContentControl>(pivot, "HeaderClipper").Visibility = Visibility.Collapsed;
            MoreVisualTreeHelper.GetObjectByName<ContentPresenter>(pivot, "LeftHeaderPresenter").Visibility = Visibility.Collapsed;
            MoreVisualTreeHelper.GetObjectByName<Button>(pivot, "PreviousButton").Visibility = Visibility.Collapsed;
            MoreVisualTreeHelper.GetObjectByName<Button>(pivot, "NextButton").Visibility = Visibility.Collapsed;
            MoreVisualTreeHelper.GetObjectByName<ContentPresenter>(pivot, "RightHeaderPresenter").Visibility = Visibility.Collapsed;
        }
    }
}