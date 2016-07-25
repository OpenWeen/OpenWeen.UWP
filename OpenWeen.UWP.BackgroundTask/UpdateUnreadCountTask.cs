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
            if (unread == null)
                return;
            UpdateUnreadHelper.UpdateUnread(unread);
        }

        private bool CheckForLogin()
        {
            try
            {
                Core.Api.Entity.AccessToken = SettingHelper.GetListSetting<string>(SettingNames.AccessToken, isThrowException: true).ToList()[Settings.SelectedUserIndex];
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}