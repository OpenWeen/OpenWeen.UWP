using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model;
using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Model;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenWeen.UWP.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public TimelineViewModel Timeline { get; } = new TimelineViewModel();
        public MentionViewModel Mention { get; } = new MentionViewModel();
        public CommentViewModel Comment { get; } = new CommentViewModel();
        public CommentMentionViewModel CommentMention { get; } = new CommentMentionViewModel();
        public UserModel User { get; private set; }

        public List<HeaderModel> Header { get; } = new List<HeaderModel>()
        {
            new HeaderModel() { Icon = Symbol.Home, Text = "主页" },
            new HeaderModel() { Icon = Symbol.Account, Text = "提及" },
            new HeaderModel() { Icon = Symbol.Comment, Text = "评论" },
            new HeaderModel() { Icon = Symbol.Comment, Text = "@的评论" },
        };

        private UnReadModel _prevUnread;

        public MainPageViewModel()
        {
            //StaticResource.UpdateUnreadCountTaskComplete += StaticResource_UpdateUnreadCountTaskComplete;
            var timer = new DispatcherTimer() { Interval = TimeSpan.FromMinutes(1d) };
            timer.Tick += Timer_Tick;
            timer.Start();
            InitUser();
            InitAllList();
        }

        private async void InitAllList()
        {
            await GetUnreadCount();
            await Timeline.Refresh();
            await Mention.Refresh();
            await Comment.Refresh();
            await CommentMention.Refresh();
        }

        private async void Timer_Tick(object sender, object e)
        {
            var unread = await GetUnreadCount();
            var builder = new StringBuilder();
            if (unread.MentionStatus > 0 && unread.MentionStatus != _prevUnread?.MentionStatus)
            {
                builder.Append($"{unread.MentionStatus} 条新@");
            }
            if (unread.Cmt > 0 && unread.Cmt != _prevUnread?.Cmt)
            {
                builder.Append($"{unread.Cmt} 条新评论");
            }
            if (unread.MentionCmt > 0 && unread.MentionCmt != _prevUnread?.MentionCmt)
            {
                builder.Append($"{unread.MentionCmt} 条新提及的评论");
            }
            if (builder.Length > 0)
            {
                ToastNotificationHelper.SendToast(builder.ToString());
            }
            _prevUnread = unread;
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

                default:
                    break;
            }
        }

        private async Task<UnReadModel> GetUnreadCount()
        {
            var unread = await Core.Api.Remind.GetUnRead(StaticResource.Uid.ToString());
            Header[1].UnreadCount = unread.MentionStatus;
            Header[2].UnreadCount = unread.Cmt;
            Header[3].UnreadCount = unread.MentionCmt;
            return unread;
        }
    }
}