using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Model;
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
        public ObservableCollection<UserListModel> UserList => new ObservableCollection<UserListModel>(Settings.AccessToken.Select((item) => new UserListModel(item)));
        public bool IsMoreInfoNotifyEnable
        {
            get { return Settings.IsMoreInfoNotifyEnable; }
            set { Settings.IsMoreInfoNotifyEnable = value; }
        }
        public int SelectedUserIndex
        {
            get { return Settings.SelectedUserIndex; }
            set { Settings.SelectedUserIndex = value; Window.Current.Content = new ExtendedSplash(null, false); }
        }
        public bool EnableWaterFall
        {
            get { return Settings.EnableWaterFall; }
            set { Settings.EnableWaterFall = value; }
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
        public int LoadCount
        {
            get { return Settings.LoadCount; }
            set { Settings.LoadCount = value; }
        }
        public bool IsMergeMentionAndComment
        {
            get { return Settings.IsMergeMentionAndComment; }
            set { Settings.IsMergeMentionAndComment = value; }
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
        public void AddUser()
        {
            Frame.Navigate(typeof(LoginPage));
        }
        public void Logout()
        {
            var tokens = Settings.AccessToken.ToList();
            tokens.RemoveAt(Settings.SelectedUserIndex);
            Settings.AccessToken = tokens;
            Settings.SelectedUserIndex = 0;
            Window.Current.Content = new ExtendedSplash(null, false);
        }

        private async Task InitUid()
        {
            StaticResource.Uid = long.Parse(await Core.Api.User.Account.GetUid());
        }

        private bool CheckForLogin()
        {
            try
            {
                Core.Api.Entity.AccessToken = SettingHelper.GetListSetting<string>(SettingNames.AccessToken, isThrowException: true).ToList()[Settings.SelectedUserIndex];
                if (string.IsNullOrEmpty(Core.Api.Entity.AccessToken))
                {
                    throw new Core.Exception.InvalidAccessTokenException();
                }
                return true;
            }
            catch (Exception e) when (e is Core.Exception.InvalidAccessTokenException || e is SettingException)
            {
                return false;
            }

        }
    }
}