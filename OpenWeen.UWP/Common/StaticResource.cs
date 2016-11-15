using System.Collections.Generic;
using System.Linq;
using OpenWeen.Core.Model;

namespace OpenWeen.UWP.Common
{
    internal static class StaticResource
    {
        private static string _emotionPattern;
        private static List<IGrouping<string, EmotionModel>> _emojiGroup;
        public static List<IGrouping<string, EmotionModel>> EmojiGroup => _emojiGroup;
        private static List<EmotionModel> _emotions;
        public static List<EmotionModel> Emotions
        {
            get { return _emotions; }
            set
            {
                _emotions = value;
                if (value != null)
                {
                    _emotionPattern = string.Join("|", value.Select(item => item.Value)).Replace("[", @"\[").Replace("]", @"\]");
                    _emojiGroup = value.GroupBy(item => string.IsNullOrEmpty(item.Category) ? "表情" : item.Category).ToList(); ;
                }
            }
        }
        public static string EmotionPattern => _emotionPattern;
        public static long Uid { get; set; }
        //public static event EventHandler UpdateUnreadCountTaskComplete;
        public static bool IsPhone => Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");

    }
}