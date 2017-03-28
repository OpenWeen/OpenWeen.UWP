using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenWeen.Core.Model;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.Storage;
using System.Net;
using System.Net.Http;

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

        public static async Task InitEmotion()
        {
            var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("emotion.json");
            if (file == null)
                return;
            var text = File.ReadAllText(file.Path);
            Emotions = JsonHelper.FromJson<List<EmotionModel>>(text);
        }
        public static async Task<bool> CheckForLogin()
        {
            try
            {
                do
                {
                    Core.Api.Entity.AccessToken = SettingHelper.GetListSetting<string>(SettingNames.AccessToken, isThrowException: true).ToList()[Settings.SelectedUserIndex];
                    if (string.IsNullOrEmpty(Core.Api.Entity.AccessToken))
                        throw new Core.Exception.InvalidAccessTokenException();
                    if (string.IsNullOrEmpty((await Core.Api.User.Account.GetLimitStatus()).Error))
                        break;
                    var list = SettingHelper.GetListSetting<string>(SettingNames.AccessToken, isThrowException: true).ToList();
                    list.RemoveAt(Settings.SelectedUserIndex);
                    Settings.SelectedUserIndex = 0;
                    SettingHelper.SetListSetting(SettingNames.AccessToken, list);
                    if (list.Count == 0)
                        return false;
                } while (true);
                return true;
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                return false;
            }
            catch (Exception e)// when (e is Core.Exception.InvalidAccessTokenException || e is SettingException)
            {
                return false;
            }
        }
    }
}