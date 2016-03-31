using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Entities;
using OpenWeen.UWP.Common.Helpers;
using OpenWeen.UWP.Model;
using OpenWeen.UWP.View;
using OpenWeen.UWP.ViewModel;
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
        public WeiboActionModel ActionModel { get; } = new WeiboActionModel();

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PostWeiboPage), new PostWeibo());
        }

        public void BackTop()
        {
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(pivot.SelectedItem as PivotItem);
            scrollViewer.ChangeView(0, 0, 1f, false);
        }

        private void WeiboList_ItemClick(object sender, Common.Controls.Events.WeiboItemClickEventArgs e)
        {
        }

        private void WeiboList_TopicClick(object sender, Common.Controls.Events.WeiboTopicClickEventArgs e)
        {
        }

        private void Ellipse_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ActionModel.UserClick(null, new Common.Controls.Events.WeiboUserClickEventArgs(StaticResource.Uid));
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
    }
}