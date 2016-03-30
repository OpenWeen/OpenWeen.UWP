using OpenWeen.UWP.Model;
using OpenWeen.UWP.ViewModel;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OpenWeen.UWP.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WeiboDetailPage : Page
    {
        public WeiboActionModel ActionModel { get; } = new WeiboActionModel();
        public WeiboDetailViewModel WeiboDetailVM { get; private set; }
        public WeiboDetailPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            WeiboDetailVM = e.Parameter as WeiboDetailViewModel;
        }
        private void Reshare_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ActionModel.Repost(sender, new Common.Controls.Events.WeiboActionEventArgs(WeiboDetailVM.Item));
        }

        private void Comment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ActionModel.Comment(sender, new Common.Controls.Events.WeiboActionEventArgs(WeiboDetailVM.Item));
        }

        private void Favor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
        }

        private void WeiboImageList_PictureClick(object sender, Common.Controls.Events.WeiboPictureClickEventArgs e)
        {
            e.DataContext = WeiboDetailVM.Item;
            ActionModel.PictureClick(sender, e);
        }
    }
}
