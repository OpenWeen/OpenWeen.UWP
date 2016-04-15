using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.Tiles;
using OpenWeen.Core.Model;
using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Model;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using OpenWeen.UWP.ViewModel.MessagePage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Tools;

namespace OpenWeen.UWP.ViewModel.MainPage
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public TimelineViewModel Timeline { get; } = new TimelineViewModel();
        public MentionViewModel Mention { get; } = new MentionViewModel();
        public CommentViewModel Comment { get; } = new CommentViewModel();
        public CommentMentionViewModel CommentMention { get; } = new CommentMentionViewModel();
        public FavorViewModel Favor { get; } = new FavorViewModel();
        public MessageUserListViewModel Message { get; } = new MessageUserListViewModel();
        public UserModel User { get; private set; }

        public List<HeaderModel> Header { get; } = new List<HeaderModel>()
        {
            new HeaderModel() { Icon = Symbol.Home, Text = "主页" },
            new HeaderModel() { Icon = Symbol.Account, Text = "提及" },
            new HeaderModel() { Icon = Symbol.Comment, Text = "评论" },
            new HeaderModel() { Icon = Symbol.Comment, Text = "@的评论" },
            new HeaderModel() { Icon = Symbol.Favorite, Text = "收藏夹" },
            new HeaderModel() { Icon = Symbol.Mail, Text = "私信" },
        };

        private UnReadModel _prevUnread;

        public MainPageViewModel()
        {
            if (Settings.NotifyDuration !=  NotifyDuration.Never)
            {
                var time = 1;
                switch (Settings.NotifyDuration)
                {
                    case NotifyDuration.ThreeMin:
                        time = 3;
                        break;
                    case NotifyDuration.FiveMin:
                        time = 5;
                        break;
                    case NotifyDuration.TenMin:
                        time = 10;
                        break;
                    case NotifyDuration.HalfHour:
                        time = 30;
                        break;
                    default:
                        break;
                }
                var timer = new BackgroundTimer() { Interval = TimeSpan.FromMinutes(time) };
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            InitUser();
            InitAllList();
        }

        private void InitAllList()
        {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            GetUnreadCount();
            Timeline.Refresh();
            Mention.Refresh();
            Comment.Refresh();
            CommentMention.Refresh();
            Favor.Refresh();
            Message.Refresh();
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        }

        private void Timer_Tick(object sender, object e)
        {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var unread = await GetUnreadCount();
                UpdateUnreadHelper.Count = Header.Sum(item => item.UnreadCount);
                if (unread == null)
                    return;
                UpdateUnreadHelper.UpdateUnread(unread);
            });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        }


        //private void StaticResource_UpdateUnreadCountTaskComplete(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        private async void InitUser()
        {
            User = await Core.Api.User.User.GetUser(StaticResource.Uid);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(User)));
        }

        public async void Refresh()
        {
            await GetUnreadCount();
            RefreshWithoutGetUnreadCount();
        }

        public async void RefreshWithoutGetUnreadCount()
        {
            switch (Header.FindIndex(item => item.IsActive))
            {
                case 0:
                    await Timeline.Refresh();
                    break;
                case 1:
                    await Mention.Refresh();
                    break;
                case 2:
                    await Comment.Refresh();
                    break;
                case 3:
                    await CommentMention.Refresh();
                    break;
                case 4:
                    await Favor.Refresh();
                    break;
                case 5:
                    await Message.Refresh();
                    break;
                default:
                    break;
            }
            UpdateUnreadHelper.Count = Header.Sum(item => item.UnreadCount);
        }

        private async Task<UnReadModel> GetUnreadCount()
        {
            try
            {
                var unread = await Core.Api.Remind.GetUnRead(StaticResource.Uid.ToString());
                if (Settings.IsMentionNotify)
                {
                    Header[1].UnreadCount = unread.MentionStatus;
                    Header[3].UnreadCount = unread.MentionCmt;
                }
                if (Settings.IsCommentNotify)
                {
                    Header[2].UnreadCount = unread.Cmt;
                }
                if (Settings.IsMessageNotify)
                {
                    Header[5].UnreadCount = unread.Dm;
                }
                return unread;
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                return null;
            }
        }
    }
}