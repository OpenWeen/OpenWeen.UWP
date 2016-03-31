using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OpenWeen.UWP.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/OpenWeen/OpenWeen.UWP"));
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(@"mailto:CosImg@outlook.com?subject=OpenWeen反馈"));
        }
    }
}