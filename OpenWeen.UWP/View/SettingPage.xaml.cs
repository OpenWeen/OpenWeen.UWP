using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class SettingPage : Page
    {
        public string BlockText
        {
            get
            {
                var list = SettingHelper.GetListSetting<string>(SettingNames.BlockText);
                return list == null ? "" : string.Join(",", list);
            }
            set
            {
                SettingHelper.SetListSetting(SettingNames.BlockText, value.Split(','));
            }
        }

        public string BlockUser
        {
            get
            {
                var list = SettingHelper.GetListSetting<long>(SettingNames.BlockUser);
                return list == null ? "" : string.Join(",", list);
            }
            set
            {
                SettingHelper.SetListSetting(SettingNames.BlockUser, value.Split(',').OfType<long>().Select(item => item));
            }
        }

        public SettingPage()
        {
            this.InitializeComponent();
        }
    }
}
