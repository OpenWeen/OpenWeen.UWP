using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.ApplicationModel.Background;

namespace OpenWeen.UWP.BackgroundTask
{
    public sealed class UpdateUnreadCountTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
                return;
            taskInstance.Canceled += (s, e) =>
            {
                Debug.WriteLine("Background task canceled");
            };
            var deferral = taskInstance.GetDeferral();
            try
            {
                await UpdateUnreadCount();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Background task error: " + e.Message);
            }
            finally
            {
                deferral.Complete();
            }
        }

        private async Task UpdateUnreadCount()
        {
            if (!CheckForLogin())
                return;
            var uid = await Core.Api.User.Account.GetUid();
            var unread = await Core.Api.Remind.GetUnRead(uid);
            if (!(unread.MentionStatus > 0 || unread.Cmt > 0 || unread.MentionCmt > 0))
                return;
            var builder = new StringBuilder();
            if (unread.MentionStatus > 0)
            {
                builder.Append($"{unread.MentionStatus} 条新@");
            }
            if (unread.Cmt > 0)
            {
                builder.Append($"{unread.Cmt} 条新评论");
            }
            if (unread.MentionCmt > 0)
            {
                builder.Append($"{unread.MentionCmt} 条新提及的评论");
            }
            if (builder.Length > 0)
            {
                ToastNotificationHelper.SendToast(builder.ToString());
            }
        }

        private bool CheckForLogin()
        {
            try
            {
                Core.Api.Entity.AccessToken = SettingHelper.GetListSetting<string>(SettingNames.AccessToken, isThrowException: true).FirstOrDefault();
                return true;
            }
            catch (SettingException)
            {
                return false;
            }
        }
    }
}