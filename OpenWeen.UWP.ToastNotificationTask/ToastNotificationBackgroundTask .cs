using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Comment;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace OpenWeen.UWP.ToastNotificationTask
{

    public sealed class ToastNotificationBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            if (!CheckForLogin())
            {
                return;
            }
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
            if (!string.IsNullOrEmpty(details?.Argument))
            {
                var def = taskInstance.GetDeferral();
                try
                {
                    var content = details.UserInput["content"] as string;
                    var argument = details.Argument.Split(';').Select(item => new KeyValuePair<string, string>(item.Split('=')[0], item.Split('=')[1])).ToDictionary(x => x.Key, x => x.Value);
                    switch (argument["item"])//TODO:actually not working
                    {
                        case nameof(MessageModel):
                            if (argument.Keys.Contains("cid"))
                            {
                                await Core.Api.Comments.PostComment(Convert.ToInt64(argument["id"]), content);
                            }
                            else
                            {
                                await Core.Api.Statuses.PostWeibo.Repost(Convert.ToInt64(argument["id"]), string.Join("", $"{content}{argument["data"]}".Take(140)));
                            }
                            break;
                        case nameof(CommentModel):
                            await Core.Api.Comments.Reply(Convert.ToInt64(argument["id"]), Convert.ToInt64(argument["cid"]), $"{argument["data"]}{content}");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Background task error: " + e.Message);
                }
                finally
                {
                    def.Complete();
                }
                // Perform tasks
            }
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
