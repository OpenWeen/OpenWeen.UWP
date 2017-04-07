using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Controls;
using OpenWeen.UWP.Model;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.Storage;
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
            get => Settings.IsMoreInfoNotifyEnable;
            set => Settings.IsMoreInfoNotifyEnable = value; 
        }
        public int SelectedUserIndex
        {
            get => Settings.SelectedUserIndex;
            set { Settings.SelectedUserIndex = value; Window.Current.Content = new ExtendedSplash(null); }
        }
        public bool EnableWaterFall
        {
            get => Settings.EnableWaterFall;
            set => Settings.EnableWaterFall = value;
        }
        public bool IsAutoOffImage
        {
            get { return Settings.IsAutoOffImage; }
            set { Settings.IsAutoOffImage = value; }
        }
        public bool IsAutoNightMode
        {
            get { return Settings.IsAutoNightMode; }
            set { Settings.IsAutoNightMode = value; }
        }
        public TimeSpan AutoNightModeOnTime
        {
            get { return Settings.AutoNightModeOnTime; }
            set { Settings.AutoNightModeOnTime = value; }
        }
        public TimeSpan AutoNightModeOffTime
        {
            get { return Settings.AutoNightModeOffTime; }
            set { Settings.AutoNightModeOffTime = value; }
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
            Window.Current.Content = new ExtendedSplash(null);
        }

        public async void RemoveEmotion()
        {
            try
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("emotion", CreationCollisionOption.OpenIfExists);
                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                var jsonFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("emotion.json", CreationCollisionOption.ReplaceExisting);
                await jsonFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                StaticResource.Emotions = null;
                Notification.Show("删除成功");
            }
            catch (UnauthorizedAccessException)
            {
                Notification.Show("删除失败");
            }
        }
        public async void DownloadEmotion()
        {
            var dialog = new SitbackAndRelaxDialog()
            {
                DialogText = "正在获取表情列表...",
                IsIndeterminate = true
            };
            dialog.ShowAsync();
            var list = (await Core.Api.Statuses.Emotions.GetEmotions()).ToList();
            dialog.DialogText = "正在下载表情图片...";
            dialog.IsIndeterminate = false;
            dialog.ProgressMaximum = list.Count;
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("emotion", CreationCollisionOption.OpenIfExists);
            using (var client = new HttpClient())
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (string.IsNullOrEmpty(item.Category))
                        item.Category = "表情";
                    var catfolder = await folder.CreateFolderAsync(item.Category, CreationCollisionOption.OpenIfExists);
                    var fileName = $"{item.Value.Replace("[", "").Replace("]","")}.jpg";
                    if (await catfolder.TryGetItemAsync(fileName) == null)
                    {
                        var file = await catfolder.CreateFileAsync(fileName);
                        using (var fileStream = (await file.OpenAsync(FileAccessMode.ReadWrite)).AsStreamForWrite())
                        using (var iconStream = (await client.GetStreamAsync(item.Url)))
                            await iconStream.CopyToAsync(fileStream);
                    }
                    //can not load the image from localcache
                    list[i].Url = $"ms-appdata:///local/emotion/{item.Category}/{fileName}";
                    dialog.ProgressValue++;
                }
            StaticResource.Emotions = list;
            var jsonFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("emotion.json", CreationCollisionOption.ReplaceExisting);
            File.WriteAllText(jsonFile.Path, JsonHelper.ToJson(list), Encoding.UTF8);
            dialog.Hide();
            Notification.Show("下载完毕");
        }
    }
}