using System;
using System.Collections.Generic;
using System.Text;
using OpenWeen.UWP.Shared.Common.Helpers;
using static OpenWeen.UWP.Shared.Common.Helpers.SettingHelper;

namespace OpenWeen.UWP.Shared.Common
{
    internal static class Settings
    {
        public static bool EnableWaterFall
        {
            get { return GetSetting(SettingNames.EnableWaterFall, false); }
            set { SetSetting(SettingNames.EnableWaterFall, value); }
        }
        public static bool IsMergeMentionAndComment
        {
            get { return GetSetting(SettingNames.IsMergeMentionAndComment, false); }
            set { SetSetting(SettingNames.IsMergeMentionAndComment, value); }
        }
        public static int LoadCount
        {
            get { return GetSetting(SettingNames.LoadCount, 20); }
            set { SetSetting(SettingNames.LoadCount, value); }
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
        public static int SelectedUserIndex
        {
            get { return GetSetting(SettingNames.SelectedUserIndex, 0); }
            set { SetSetting(SettingNames.SelectedUserIndex, value); }
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

        public static bool IsNightMode
        {
            get { return GetSetting(SettingNames.IsNightMode, false); }
            set { SetSetting(SettingNames.IsNightMode, value); }
        }

        public static bool IsOffImage
        {
            get { return GetSetting(SettingNames.IsOffImage, false); }
            set { SetSetting(SettingNames.IsOffImage, value); }
        }
        
        public static bool IsAutoNightMode
        {
            get { return GetSetting(SettingNames.IsAutoNightMode, false); }
            set { SetSetting(SettingNames.IsAutoNightMode, value); }
        }

        public static bool IsAutoOffImage
        {
            get { return GetSetting(SettingNames.IsAutoOffImage, false); }
            set { SetSetting(SettingNames.IsAutoOffImage, value); }
        }
        public static TimeSpan AutoNightModeOnTime
        {
            get { return GetSetting(SettingNames.AutoNightModeOnTime, new TimeSpan(22, 00, 00)); }
            set { SetSetting(SettingNames.AutoNightModeOnTime, value); }
        }
        public static TimeSpan AutoNightModeOffTime
        {
            get { return GetSetting(SettingNames.AutoNightModeOnTime, new TimeSpan(7, 00, 00)); }
            set { SetSetting(SettingNames.AutoNightModeOnTime, value); }
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
