using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Status;
using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Model;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.UI.Core;
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
        public MergeMessageViewModel MergeMessage { get; } = new MergeMessageViewModel();
        public UserModel User { get; private set; }
        public List<GroupModel> Groups { get; } = new List<GroupModel>()
        {
            new GroupModel() { ID = -1, Name = "全部分组" }
        };
        private int _groupSelectedIndex = 0;
        private BackgroundTimer _timer;

        public int GroupSelectedIndex
        {
            get { return _groupSelectedIndex; }
            set
            {
                _groupSelectedIndex = value;
                Timeline.SetGroupAndRefresh(Groups[value]);
            }
        }


        public List<HeaderModel> Header { get; private set; } 

        public static MainPageViewModel Instance { get; } = new MainPageViewModel();

        private MainPageViewModel()
        {
        }

        internal void Initialization()
        {
            if (Settings.NotifyDuration != NotifyDuration.Never)
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
                if (_timer == null)
                {
                    _timer = new BackgroundTimer() { Interval = TimeSpan.FromMinutes(time) };
                    _timer.Tick += Timer_Tick;
                    _timer.Start();
                }
            }
            InitHeader();
            InitUser();
            InitAllList();
        }

        internal void InitHeader()
        {
            if (Settings.IsMergeMentionAndComment)
            {
                Header = new List<HeaderModel>
                {
                    new HeaderModel() { Icon = Symbol.Home, Text = "主页" },
                    new HeaderModel() { Icon = Symbol.Message, Text = "消息" },
                    new HeaderModel() { Icon = Symbol.Favorite, Text = "收藏" },
                    new HeaderModel() { Icon = Symbol.Mail, Text = "私信" },
                };
            }
            else
            {
                Header = new List<HeaderModel>
                {
                    new HeaderModel() { Icon = Symbol.Home, Text = "主页" },
                    new HeaderModel() { Icon = Symbol.Account, Text = "提及" },
                    new HeaderModel() { Icon = Symbol.Comment, Text = "评论" },
                    new HeaderModel() { Icon = Symbol.Comment, Text = "@评论" },
                    new HeaderModel() { Icon = Symbol.Favorite, Text = "收藏" },
                    new HeaderModel() { Icon = Symbol.Mail, Text = "私信" },
                };
            }
        }

        internal async void InitAllList()
        {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            GetUnreadCount();
            Timeline.Refresh();
            if (Settings.IsMergeMentionAndComment)
            {
                MergeMessage.Refresh();
            }
            else
            {
                Mention.Refresh();
                Comment.Refresh();
                CommentMention.Refresh();
            }
            Favor.Refresh();
            Message.Refresh();
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            try
            {
                (await Core.Api.Friendships.Groups.GetGroups()).Lists.ForEach(item => Groups.Add(item));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Groups)));
            }
            catch (Exception ex) when (ex is WebException || ex is HttpRequestException || ex is TaskCanceledException || ex is NullReferenceException)
            {
            }
        }

        private void Timer_Tick(object sender, object e)
        {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    var unread = await GetUnreadCount();
                    if (unread == null)
                        return;
                    UpdateUnreadHelper.UpdateUnread(unread);
                    UpdateUnreadHelper.Count = Header.Sum(item => item.UnreadCount);
                }
                catch (Exception)
                {

                }
            });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        }

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
            if (Settings.IsMergeMentionAndComment)
            {
                switch (Header.FindIndex(item => item.IsActive))
                {
                    case 0:
                        await Timeline.Refresh();
                        break;
                    case 1:
                        await MergeMessage.Refresh();
                        Header[1].UnreadCount = 0;
                        break;
                    case 2:
                        await Favor.Refresh();
                        Header[2].UnreadCount = 0;
                        break;
                    case 3:
                        await Message.Refresh();
                        Header[3].UnreadCount = 0;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (Header.FindIndex(item => item.IsActive))
                {
                    case 0:
                        await Timeline.Refresh();
                        break;
                    case 1:
                        await Mention.Refresh();
                        Header[1].UnreadCount = 0;
                        break;
                    case 2:
                        await Comment.Refresh();
                        Header[2].UnreadCount = 0;
                        break;
                    case 3:
                        await CommentMention.Refresh();
                        Header[3].UnreadCount = 0;
                        break;
                    case 4:
                        await Favor.Refresh();
                        break;
                    case 5:
                        await Message.Refresh();
                        Header[5].UnreadCount = 0;
                        break;
                    default:
                        break;
                }
            }
            UpdateUnreadHelper.Count = Header.Sum(item => item.UnreadCount);
        }

        private async Task<UnReadModel> GetUnreadCount()
        {
            try
            {
                var unread = await Core.Api.Remind.GetUnRead(StaticResource.Uid.ToString());
                if (Settings.IsMergeMentionAndComment)
                {
                    Header[1].UnreadCount = 0;
                    if (Settings.IsMentionNotify)
                    {
                        Header[1].UnreadCount += unread.MentionStatus;
                        Header[1].UnreadCount += unread.MentionCmt;
                    }
                    if (Settings.IsCommentNotify)
                    {
                        Header[1].UnreadCount += unread.Cmt;
                    }
                    if (Settings.IsMessageNotify)
                    {
                        Header[3].UnreadCount = unread.Dm;
                    }
                    return unread;
                }
                else
                {
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
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                return null;
            }
        }
    }
}