using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Notifications;

namespace OpenWeen.UWP.Shared.Common.Helpers
{
    internal static class ToastNotificationHelper
    {
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
    }
}
