using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Status;
using Windows.UI.Notifications;
using System.Reflection;
using OpenWeen.Core.Model.Comment;

namespace OpenWeen.UWP.Shared.Common.Helpers
{
    internal static class ToastNotificationHelper
    {
        public static void SendToast(string content, int badgeCount)
        {
            SendToast(content);
            UpdateBadgeCount(badgeCount);

        }

        public static void SendToast(string content)
        {
            var toastTemplate = ToastTemplateType.ToastText01;
            var xml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            var text = xml.CreateTextNode(content);
            var elements = xml.GetElementsByTagName("text");
            elements[0].AppendChild(text);
            var toast = new ToastNotification(xml);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);
        }


        public static void SendToast(BaseModel msg)
        {
            string id = msg is CommentModel ? msg.GetType().GetRuntimeProperty("Status").GetValue(msg).GetType().GetRuntimeProperty("ID").GetValue(msg.GetType().GetRuntimeProperty("Status").GetValue(msg)).ToString() : msg.ID.ToString();
            string cid = msg is CommentModel ? msg.ID.ToString() : null;
            string data = msg is CommentModel ? $"回复@{msg.User.ScreenName}:" : msg.GetType().GetRuntimeProperty("RetweetedStatus").GetValue(msg) == null ? "" : $"//@{msg.User.Name}:{msg.Text}";
            string XML =
"<toast>" +
  "<visual>" +
    "<binding template=\"ToastGeneric\">" +
      $"<text>{msg.User.ScreenName}</text>" +
      $"<text>{msg.Text}</text>" +
      $"<image placement=\"appLogoOverride\" src=\"{msg.User.ProfileImageUrl}\" />" +
    "</binding>" +
  "</visual>" +
  "<actions>" +
    "<input id=\"content\" type=\"text\" placeHolderContent=\"回复\" />" +
    (msg is MessageModel ? $"<action activationType=\"background\" content=\"转发\" arguments=\"item={msg.GetType().Name};id={id};data={data}\" hint-inputId=\"转发微博\"/>" : null) +
    $"<action activationType=\"background\" content=\"回复\" arguments=\"item={msg.GetType().Name};id={id};cid={cid};data={data}\" hint-inputId=\"转发微博\"/>" +
  "</actions>" +
"</toast>";
            var xml = new Windows.Data.Xml.Dom.XmlDocument();
            xml.LoadXml(XML);
            var toast = new ToastNotification(xml);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);
        }


        public static void UpdateBadgeCount(int badgeCount)
        {
            var type = BadgeTemplateType.BadgeNumber;
            var xml = BadgeUpdateManager.GetTemplateContent(type);
            var elements = xml.GetElementsByTagName("badge");
            var element = elements[0] as Windows.Data.Xml.Dom.XmlElement;
            element.SetAttribute("value", badgeCount.ToString());
            var updator = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
            var notification = new BadgeNotification(xml);
            updator.Update(notification);
        }
    }
}