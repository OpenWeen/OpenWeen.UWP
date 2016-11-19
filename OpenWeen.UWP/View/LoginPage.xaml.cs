using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenWeen.Core.Helper;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Controls;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.Security.Authentication.Web;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using Windows.UI.Core;
using OpenWeen.UWP.Common.Entities;

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
            var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
            {
                try
                {
                    //SSMjExMTYwNjc5OjoxZTZlMzNkYjA4ZjkxOTIzMDZjNGFmYTBhNjFhZDU2Yzo6aHR0cDovL29hdXRoLndlaWNvLmNjOjplbWFpbCxkaXJlY3RfbWVzc2FnZXNfcmVhZCxkaXJlY3RfbWVzc2FnZXNfd3JpdGUsZnJpZW5kc2hpcHNfZ3JvdXBzX3JlYWQsZnJpZW5kc2hpcHNfZ3JvdXBzX3dyaXRlLHN0YXR1c2VzX3RvX21lX3JlYWQsZm9sbG93X2FwcF9vZmZpY2lhbF9taWNyb2Jsb2csaW52aXRhdGlvbl93cml0ZTo6Y29tLmVpY28ud2VpY286OkVFEE
                    var text = await dataPackageView.GetTextAsync();
                    var data = LoginDataHelper.Decode(text.Trim());
                    AppID_TB.Text = data[0];
                    AppSecret_TB.Text = data[1];
                    RedirectUri_TB.Text = data[2];
                    Scope_TB.Text = data[3];
                    PackageName_TB.Text = data[4];
                    e.Handled = true;
                }
                catch
                {
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var requri = new Uri($"https://api.weibo.com/oauth2/authorize?client_id={AppID_TB.Text}&response_type=token&display=mobile&redirect_uri={RedirectUri_TB.Text}&key_hash={AppSecret_TB.Text}{(string.IsNullOrEmpty(PackageName_TB.Text) ? "" : $"&packagename={PackageName_TB.Text}")}&scope={Scope_TB.Text}");
                var callbackuri = new Uri(RedirectUri_TB.Text);
                var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, requri, callbackuri);
                if (result.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var regex = Regex.Match(result.ResponseData, "access_token=(.*)\\&remind_in=([0-9]*)");
                    var token = regex.Groups[1].Value;
                    if (string.IsNullOrEmpty(token))
                    {
                        throw new UnauthorizedAccessException();
                    }
                    SettingHelper.SetListSetting(SettingNames.AccessToken, new[] { token }, SetListSettingOption.AddIfExists);
                    Settings.SelectedUserIndex = Settings.AccessToken.Count() - 1;
                    Core.Api.Entity.AccessToken = token;
                    Window.Current.Content = new ExtendedSplash(null);
                }
            }
            catch (UriFormatException)
            {
                await new MessageDialog("请输入正确的参数内容").ShowAsync();
                return;
            }
            catch (FileNotFoundException)
            {
                //user can not connect to the auth page and close the auth page
                return;
            }
            catch (UnauthorizedAccessException)
            {
                await new MessageDialog("登陆失败").ShowAsync();
                return;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Frame.BackStack.Clear();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            var cahcesize = Frame.CacheSize;
            Frame.CacheSize = 0;
            Frame.CacheSize = cahcesize;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var content = new Windows.ApplicationModel.DataTransfer.DataPackage();
            content.SetText("SSMjExMTYwNjc5OjoxZTZlMzNkYjA4ZjkxOTIzMDZjNGFmYTBhNjFhZDU2Yzo6aHR0cDovL29hdXRoLndlaWNvLmNjOjplbWFpbCxkaXJlY3RfbWVzc2FnZXNfcmVhZCxkaXJlY3RfbWVzc2FnZXNfd3JpdGUsZnJpZW5kc2hpcHNfZ3JvdXBzX3JlYWQsZnJpZW5kc2hpcHNfZ3JvdXBzX3dyaXRlLHN0YXR1c2VzX3RvX21lX3JlYWQsZm9sbG93X2FwcF9vZmZpY2lhbF9taWNyb2Jsb2csaW52aXRhdGlvbl93cml0ZTo6Y29tLmVpY28ud2VpY286OkVFEE");
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(content);
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://gist.github.com/PeterCxy/3085799055f63c63c911"));
        }
    }
}