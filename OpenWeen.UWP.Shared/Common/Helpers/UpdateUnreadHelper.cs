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

        public static void UpdateUnread(UnReadModel unread)
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
            if (unread.Dm > 0 && unread.Dm != _prevUnread.Dm && Settings.IsMessageNotify)
            {
                builder.Append($"{unread.Dm} 条新私信");
                _count += unread.Dm;
            }
            if (builder.Length > 0)
            {
                ToastNotificationHelper.SendToast(builder.ToString(), Count);
            }
            _prevUnread = unread;
        }
    }
}
