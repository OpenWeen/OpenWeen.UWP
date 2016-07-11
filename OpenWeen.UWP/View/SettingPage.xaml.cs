using System;
using System.Linq;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OpenWeen.UWP.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public bool IsMoreInfoNotifyEnable
        {
            get { return Settings.IsMoreInfoNotifyEnable; }
            set { Settings.IsMoreInfoNotifyEnable = value; }
        }
        public string BlockText
        {
            get
            {
                var list = Settings.BlockText;
                return list == null ? "" : string.Join(",", list);
            }
            set
            {
                Settings.BlockText = value.Split(',');
            }
        }
        
        public string BlockUser
        {
            get
            {
                var list = Settings.BlockUser;
                return list == null ? "" : string.Join(",", list);
            }
            set
            {
                Settings.BlockUser = value.Split(',').OfType<long>().Select(item => item);
            }
        }
        public int NotifyDurationIndex
        {
            get
            {
                return (int)Settings.NotifyDuration;
            }
            set
            {
                Settings.NotifyDuration = (NotifyDuration)value;
            }
        }
        public double ImageSize
        {
            get
            {
                return Settings.ImageSize;
            }
            set
            {
                Settings.ImageSize = value;
            }
        }

        public bool IsMentionNotify
        {
            get
            {
                return Settings.IsMentionNotify;
            }
            set
            {
                Settings.IsMentionNotify = value;
            }
        }
        public bool IsCommentNotify
        {
            get
            {
                return Settings.IsCommentNotify;
            }
            set
            {
                Settings.IsCommentNotify = value;
            }
        }
        public bool IsMessageNotify
        {
            get
            {
                return Settings.IsMessageNotify;
            }
            set
            {
                Settings.IsMessageNotify = value;
            }
        }
        public bool IsFollowerNotify
        {
            get
            {
                return Settings.IsFollowerNotify;
            }
            set
            {
                Settings.IsFollowerNotify = value;
            }
        }
        public bool IsNightMode
        {
            get { return Settings.IsNightMode; }
            set { Settings.IsNightMode = value; }
        }
        public bool IsOffImage
        {
            get { return Settings.IsOffImage; }
            set { Settings.IsOffImage = value; }
        }
        public SettingPage()
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
        public void Crash()
        {
            throw new Exception("爆炸了！");
        }
    }
}