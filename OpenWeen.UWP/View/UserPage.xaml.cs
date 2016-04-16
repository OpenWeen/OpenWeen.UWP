using OpenWeen.UWP.Model;
using OpenWeen.UWP.ViewModel;
using OpenWeen.UWP.ViewModel.MessagePage;
using OpenWeen.UWP.ViewModel.UserPage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OpenWeen.UWP.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserPage : Page
    {
        public UserPageViewModel UserPageVM { get; private set; }
        public WeiboActionModel ActionModel { get; } = WeiboActionModel.Instance;

        public UserPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UserPageVM = e.Parameter as UserPageViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var menu = Resources["SettingFlyout"] as MenuFlyout;
            menu.ShowAt(sender as FrameworkElement);
        }

        public void Follower()
        {
            if (UserPageVM.User == null)
                return;
            Frame.Navigate(typeof(UserListPage), new FollowerListViewModel(UserPageVM.User.ID));
        }
        public void Friend()
        {
            if (UserPageVM.User == null)
                return;
            Frame.Navigate(typeof(UserListPage), new FriendListViewModel(UserPageVM.User.ID));
        }
        public void SendMessage()
        {
            if (UserPageVM.User == null || UserPageVM.IsMe)
                return;
            Frame.Navigate(typeof(MessagePage), new MessagePageViewModel(UserPageVM.User.ID, UserPageVM.User.ScreenName));
        }
    }
}