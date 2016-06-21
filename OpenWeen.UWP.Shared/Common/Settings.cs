using System;
using System.Collections.Generic;
using System.Text;
using OpenWeen.UWP.Shared.Common.Helpers;
using static OpenWeen.UWP.Shared.Common.Helpers.SettingHelper;

namespace OpenWeen.UWP.Shared.Common
{
    internal static class Settings
    {
        public static double ImageSize
        {
            get
            {
                return GetSetting(SettingNames.ImageSize, 100d);
            }
            set
            {
                SetSetting(SettingNames.ImageSize, value);
            }
        }
        public static IEnumerable<long> BlockUser
        {
            get
            {
                return GetListSetting<long>(SettingNames.BlockUser);
            }
            set
            {
                SetListSetting(SettingNames.BlockUser, value);
            }
        }
        public static IEnumerable<string> BlockText
        {
            get
            {
                return GetListSetting<string>(SettingNames.BlockText);
            }
            set
            {
                SetListSetting(SettingNames.BlockText, value);
            }
        }
        public static IEnumerable<string> AccessToken
        {
            get
            {
                return GetListSetting<string>(SettingNames.AccessToken);
            }
            set
            {
                SetListSetting(SettingNames.AccessToken, value);
            }
        }
        public static NotifyDuration NotifyDuration
        {
            get
            {
                return (NotifyDuration)GetSetting(SettingNames.NotifyDuration, 0);
            }
            set
            {
                SetSetting(SettingNames.NotifyDuration, value);
            }
        }
        public static bool IsMentionNotify
        {
            get
            {
                return GetSetting(SettingNames.IsMentionNotify, true);
            }
            set
            {
                SetSetting(SettingNames.IsMentionNotify, value);
            }
        }
        public static bool IsCommentNotify
        {
            get
            {
                return GetSetting(SettingNames.IsCommentNotify, true);
            }
            set
            {
                SetSetting(SettingNames.IsCommentNotify, value);
            }
        }
        public static bool IsMessageNotify
        {
            get
            {
                return GetSetting(SettingNames.IsMessageNotify, true);
            }
            set
            {
                SetSetting(SettingNames.IsMessageNotify, value);
            }
        }
        public static bool IsFollowerNotify
        {
            get
            {
                return GetSetting(SettingNames.IsFollowerNotify, true);
            }
            set
            {
                SetSetting(SettingNames.IsFollowerNotify, value);
            }
        }

        public static bool IsMoreInfoNotifyEnable
        {
            get
            {
                return GetSetting(SettingNames.IsMoreInfoNotifyEnable, false);
            }
            set
            {
                SetSetting(SettingNames.IsMoreInfoNotifyEnable, value);
            }
        }

    }
    internal enum NotifyDuration
    {
        OneMin = 0,
        ThreeMin,
        FiveMin,
        TenMin,
        HalfHour,
        Never,
    }
}
