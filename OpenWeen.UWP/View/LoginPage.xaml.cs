using OpenWeen.Core.Helper;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OpenWeen.UWP.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void TextBox_Paste(object sender, TextControlPasteEventArgs e)
        {
            e.Handled = true;
            var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
            {
                try
                {
                    //SSMjExMTYwNjc5OjoxZTZlMzNkYjA4ZjkxOTIzMDZjNGFmYTBhNjFhZDU2Yzo6aHR0cDovL29hdXRoLndlaWNvLmNjOjplbWFpbCxkaXJlY3RfbWVzc2FnZXNfcmVhZCxkaXJlY3RfbWVzc2FnZXNfd3JpdGUsZnJpZW5kc2hpcHNfZ3JvdXBzX3JlYWQsZnJpZW5kc2hpcHNfZ3JvdXBzX3dyaXRlLHN0YXR1c2VzX3RvX21lX3JlYWQsZm9sbG93X2FwcF9vZmZpY2lhbF9taWNyb2Jsb2csaW52aXRhdGlvbl93cml0ZTo6Y29tLmVpY28ud2VpY286OkVFEE
                    var text = await dataPackageView.GetTextAsync();
                    var data = LoginDataHelper.Decode(text);
                    AppID_TB.Text = data[0];
                    AppSecret_TB.Text = data[1];
                    RedirectUri_TB.Text = data[2];
                    Scope_TB.Text = data[3];
                    PackageName_TB.Text = data[4];
                }
                catch (Exception)
                {
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri($"https://api.weibo.com/oauth2/authorize?client_id={AppID_TB.Text}&response_type=token&display=mobile&redirect_uri={RedirectUri_TB.Text}&key_hash={AppSecret_TB.Text}{(string.IsNullOrEmpty(PackageName_TB.Text)?"": $"&packagename={PackageName_TB.Text}")}&scope={Scope_TB.Text}"), new Uri(RedirectUri_TB.Text));

            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var regex = Regex.Match(result.ResponseData, "access_token=(.*)\\&remind_in=([0-9]*)");
                var token = regex.Groups[1].Value;
                SettingHelper.SetListSetting(SettingNames.AccessToken, new[] { token }, SetListSettingOption.AddIdExists);
                Core.Api.Entity.AccessToken = token;
                await InitEmotion();
                await InitUid();
                Frame.Navigate(typeof(MainPage));
                while (Frame.BackStack.Count > 0)
                {
                    Frame.BackStack.RemoveAt(0);
                }
            }
        }
        private async Task InitUid()
        {
            StaticResource.Uid = long.Parse(await Core.Api.User.Account.GetUid());
        }

        private static async System.Threading.Tasks.Task InitEmotion()
        {
            StaticResource.Emotions = (await Core.Api.Statuses.Emotions.GetEmotions()).ToList();
            StaticResource.EmotionPattern = string.Join("|", StaticResource.Emotions.Select(item => item.Value)).Replace("[", @"\[").Replace("]", @"\]");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://gist.github.com/PeterCxy/3085799055f63c63c911"));
        }
    }
}
