using Windows.UI.Notifications;

namespace OpenWeen.UWP.Shared.Common.Helpers
{
    internal static class ToastNotificationHelper
    {
        public static void SendToast(string content, int badgeCount)
        {
            var toastTemplate = ToastTemplateType.ToastText01;
            var xml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            var text = xml.CreateTextNode(content);
            var elements = xml.GetElementsByTagName("text");
            elements[0].AppendChild(text);
            var toast = new ToastNotification(xml);
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);
            UpdateBadgeCount(badgeCount);
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