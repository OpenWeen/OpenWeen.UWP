using System;
using System.Collections.Generic;
using System.Text;
using OpenWeen.Core.Model;

namespace OpenWeen.UWP.Shared.Common.Helpers
{
    internal static class UpdateUnreadHelper
    {
        private static int _count;

        public static int Count//TODO:BE CAREFUL
        {
            get { return _count; }
            set
            {
                _count = value;
                ToastNotificationHelper.UpdateBadgeCount(value);
            }
        }


        private static UnReadModel _prevUnread;

        public static async void UpdateUnread(UnReadModel unread)
        {
            if (unread == null)
                return;
            if (Settings.IsMoreInfoNotifyEnable)
                await NotifyUnread(unread);
            else
                NotifyUnreadAllInOnce(unread);
            _prevUnread = unread;
        }

        private static async System.Threading.Tasks.Task NotifyUnread(UnReadModel unread)
        {
            if (unread.MentionStatus > 0 && unread.MentionStatus != _prevUnread?.MentionStatus && Settings.IsMentionNotify)
            {
                try
                {
                    Core.Api.Remind.ClearUnRead(Core.Api.RemindType.MentionStatus);
                    var items = await Core.Api.Statuses.Mentions.GetMentions(count: unread.MentionStatus);
                    items.Statuses.ForEach(item => ToastNotificationHelper.SendToast(item));
                }
                catch (Exception)
                {
                    ToastNotificationHelper.SendToast($"{unread.MentionStatus} 条新@");
                }
            }
            if (unread.Cmt > 0 && unread.Cmt != _prevUnread?.Cmt && Settings.IsCommentNotify)
            {
                try
                {
                    Core.Api.Remind.ClearUnRead(Core.Api.RemindType.Cmt);
                    var items = await Core.Api.Comments.GetCommentToMe(count: unread.Cmt);
                    items.Comments.ForEach(item => ToastNotificationHelper.SendToast(item));
                }
                catch (Exception)
                {
                    ToastNotificationHelper.SendToast($"{unread.Cmt} 条新评论");
                }
            }
            if (unread.MentionCmt > 0 && unread.MentionCmt != _prevUnread?.MentionCmt && Settings.IsMentionNotify)
            {
                try
                {
                    Core.Api.Remind.ClearUnRead(Core.Api.RemindType.MentionCmt);
                    var items = await Core.Api.Comments.GetCommentMentions(count: unread.MentionCmt);
                    items.Comments.ForEach(item => ToastNotificationHelper.SendToast(item));
                }
                catch (Exception)
                {
                    ToastNotificationHelper.SendToast($"{unread.MentionCmt} 条新提及的评论");
                }
            }
            if (unread.Follower > 0 && unread.Follower != _prevUnread?.Follower && Settings.IsFollowerNotify)
            {
                ToastNotificationHelper.SendToast($"{unread.Follower} 个新粉丝");
                Count += unread.Follower;
            }
            if (unread.Dm > 0 && unread.Dm != _prevUnread.Dm && Settings.IsMessageNotify)
            {
                ToastNotificationHelper.SendToast($"{unread.Dm} 条新私信");
                Count += unread.Dm;
            }
        }

        private static void NotifyUnreadAllInOnce(UnReadModel unread)
        {
            var builder = new StringBuilder();
            if (unread.MentionStatus > 0 && unread.MentionStatus != _prevUnread?.MentionStatus && Settings.IsMentionNotify)
            {
                builder.Append($"{unread.MentionStatus} 条新@");
                _count += unread.MentionStatus;
            }
            if (unread.Cmt > 0 && unread.Cmt != _prevUnread?.Cmt && Settings.IsCommentNotify)
            {
                builder.Append($"{unread.Cmt} 条新评论");
                _count += unread.Cmt;
            }
            if (unread.MentionCmt > 0 && unread.MentionCmt != _prevUnread?.MentionCmt && Settings.IsMentionNotify)
            {
                builder.Append($"{unread.MentionCmt} 条新提及的评论");
                _count += unread.MentionCmt;
            }
            if (unread.Follower > 0 && unread.Follower != _prevUnread?.Follower && Settings.IsFollowerNotify)
            {
                builder.Append($"{unread.Follower} 个新粉丝");
                _count += unread.Follower;
            }
            if (unread.Dm > 0 && unread.Dm != _prevUnread?.Dm && Settings.IsMessageNotify)
            {
                builder.Append($"{unread.Dm} 条新私信");
                _count += unread.Dm;
            }
            if (builder.Length > 0)
            {
                ToastNotificationHelper.SendToast(builder.ToString(), Count);
            }
        }
    }
}
