using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Model;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.UI.Popups;

namespace OpenWeen.UWP.ViewModel.UserPage
{
    public class UserPageViewModel : INotifyPropertyChanged
    {
        public UserTimelineViewModel UserTimeline { get; private set; }
        public UserImageTimelineViewModel UserImageTimeline { get; private set; }
        public UserModel User { get; private set; }
        public FollowModel Follow { get; } = new FollowModel();
        public object UidOrUserName { get; }
        public bool IsBlocked { get; private set; }
        public bool IsMe => StaticResource.Uid == User?.ID;
        public string BlockState => Settings.BlockUser?.Contains(User?.ID ?? -1) == true ? "已屏蔽" : "屏蔽";
        protected bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                _isLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            }
        }

        public double ScrollViewerPosition { get; internal set; }

        public UserPageViewModel(long uid)
        {
            UidOrUserName = uid;
            Init();
        }

        public UserPageViewModel(string userName)
        {
            UidOrUserName = userName;
            Init();
        }

        public UserPageViewModel(UserModel user)
        {
            User = user;
            InitList();
        }

        public UserPageViewModel(object uidOrUserName)
        {
            if (!(uidOrUserName is string || uidOrUserName is long))
                throw new ArgumentException("parameter must be string or long");
            UidOrUserName = uidOrUserName;
            Init();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Init()
        {
            IsLoading = true;
            try
            {
                if (UidOrUserName is string)
                {
                    User = await Core.Api.User.User.GetUser(UidOrUserName as string);
                }
                else if (UidOrUserName is long)
                {
                    User = await Core.Api.User.User.GetUser((long)UidOrUserName);
                }
                else
                {
                    throw new ArgumentException("parameter must be string or long");
                }
            }
            catch (WebException)
            {
                await new MessageDialog("无效的用户").ShowAsync();
            }
            await InitList();
            IsLoading = false;
        }

        private async System.Threading.Tasks.Task InitList()
        {
            //Fuck weibo does not allow weico to check the block state
            //IsBlocked = await Core.Api.Blocks.IsBlocked(User.ID);
            UserTimeline = new UserTimelineViewModel(User.ID);
            await UserTimeline.Refresh();
            //UserImageTimeline = new UserImageTimelineViewModel(User.ID);
            //await UserImageTimeline.Refresh();
            Follow.SetState(User.Following, User.FollowMe, IsBlocked);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlockState)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMe)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(User)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserTimeline)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserImageTimeline)));
        }

        public void AddBlock()
        {
            if (User == null || IsMe)
                return;

            if (Settings.BlockUser?.Contains(User.ID) == true)
            {
                var list = Settings.BlockUser.ToList();
                list.Remove(User.ID);
                Settings.BlockUser = list;
            }
            else
            {
                SettingHelper.SetListSetting(SettingNames.BlockUser, new[] { User.ID }, SetListSettingOption.AddIfExists);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlockState)));
        }

        public async void ChangeFollow()
        {
            if (IsLoading || User == null) return;
            IsLoading = true;
            if (User.Following)
            {
                await Core.Api.Friendships.Friends.UnFollow(User.ID);
            }
            else
            {
                await Core.Api.Friendships.Friends.Follow(User.ID);
            }
            Follow.SetState(!User.Following, User.FollowMe, IsBlocked);
            User.Following = !User.Following;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(User)));
            IsLoading = false;
        }

        public async void AddWeiboBlock()
        {
            if (User == null || IsMe)
                return;
            await Core.Api.Blocks.AddBlock(User.ID);
        }
    }
}