using OpenWeen.Core.Model.Status;
using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common.Controls.Events;
using OpenWeen.UWP.Model;
using OpenWeen.UWP.ViewModel;
using OpenWeen.UWP.ViewModel.DetailPage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OpenWeen.UWP.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WeiboDetailPage : Page
    {
        public WeiboActionModel ActionModel { get; } = WeiboActionModel.Instance;
        public WeiboDetailViewModel ViewModel { get; private set; }

        public WeiboDetailPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as WeiboDetailViewModel;
        }

        private void Reshare_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ActionModel.Repost(sender, new WeiboActionEventArgs(ViewModel.Item.Result));
        }

        private void Comment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ActionModel.Comment(sender, new WeiboActionEventArgs(ViewModel.Item.Result));
        }

        private void Favor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ActionModel.Favor(this, new WeiboActionEventArgs(ViewModel.Item.Result));
        }

        private void WeiboImageList_PictureClick(object sender, WeiboPictureClickEventArgs e)
        {
            ActionModel.PictureClick(sender, e);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var context = (e.OriginalSource as FrameworkElement).DataContext;
            if (context is UserModel)
            {
                e.Handled = true;
                ActionModel.UserClick(this, new WeiboUserClickEventArgs((context as UserModel).ID));
            }
            else if (context is MessageModel)
            {
                e.Handled = true;
                ActionModel.ItemClick(this, new WeiboItemClickEventArgs(context as MessageModel));
            }
        }

        private void Like_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ActionModel.Like(sender, new WeiboActionEventArgs(ViewModel.Item.Result));
        }
    }
}