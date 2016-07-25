using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace OpenWeen.UWP.Shared.Common.Helpers
{
    internal class SettingException : Exception
    {
        public SettingException()
        {
        }

        public SettingException(string message) : base(message)
        {
        }

        public SettingException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    internal static class SettingHelper
    {
        public static void SetSetting<T>(string settingName, T setValue)
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(settingName))
            {
                settings.Values.Remove(settingName);
            }
            settings.Values.Add(settingName, setValue);
        }

        public static T GetSetting<T>(string settingName, T defaultValue = default(T), bool isThrowException = false)
        {
            var settings = ApplicationData.Current.LocalSettings;
            T chackValue = defaultValue;
            if (settings.Values.ContainsKey(settingName))
            {
                chackValue = (T)settings.Values[settingName];
            }
            else
            {
                if (isThrowException)
                {
                    throw new SettingException("Can not Find " + settingName + " value");
                }
            }
            return chackValue;
        }

        public static void SetListSetting<T>(string settingName, IEnumerable<T> values, SetListSettingOption option = SetListSettingOption.ReplaceExisting)
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(settingName))
            {
                switch (option)
                {
                    case SetListSettingOption.ReplaceExisting:
                        settings.Values.Remove(settingName);
                        break;
                    case SetListSettingOption.FailIfExists:
                        throw new SettingException($"{settingName} already exist");
                    case SetListSettingOption.AddIfExists:
                        values = GetListSetting<T>(settingName).Concat(values);
                        settings.Values.Remove(settingName);
                        break;
                }
            }
            if (values.Count() != 0)
            {
                settings.Values.Add(settingName, values.ToArray());
            }
        }

        public static IEnumerable<T> GetListSetting<T>(string settingName, bool isThrowException = false)
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(settingName))
            {
                return ((T[])ApplicationData.Current.LocalSettings.Values[settingName]);
            }
            else
            {
                if (isThrowException)
                {
                    throw new SettingException("Can not Find " + settingName + " value");
                }
            }
            return null;
        }
    }

    internal enum SetListSettingOption
    {
        ReplaceExisting,
        FailIfExists,
        AddIfExists
    }
}