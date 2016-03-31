using System;
using System.Linq;
using OpenWeen.Core.Model.Comment;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OpenWeen.UWP.Common.Converter
{
    public class BlockToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageModel)
            {
                var data = value as MessageModel;
                return SettingHelper.GetListSetting<long>(SettingNames.BlockUser)?.Contains(data.User.ID) == true ||
                    (data.RetweetedStatus?.User?.ID != null && SettingHelper.GetListSetting<long>(SettingNames.BlockUser)?.Contains(data.RetweetedStatus.User.ID) == true) ||
                    (SettingHelper.GetListSetting<string>(SettingNames.BlockText)?.Any(item => string.IsNullOrEmpty(item) ? false : data.Text.Contains(item)) == true) ||
                    (data.RetweetedStatus?.Text != null && SettingHelper.GetListSetting<string>(SettingNames.BlockText)?.Any(item => string.IsNullOrEmpty(item) ? false : data.RetweetedStatus.Text.Contains(item)) == true) ?
                    Visibility.Collapsed : Visibility.Visible;
            }
            else if (value is CommentModel)
            {
                var data = value as CommentModel;
                return SettingHelper.GetListSetting<long>(SettingNames.BlockUser)?.Contains(data.User.ID) == true ||
                    (data.ReplyComment?.User?.ID != null && SettingHelper.GetListSetting<long>(SettingNames.BlockUser)?.Contains(data.ReplyComment.User.ID) == true) ||
                    (SettingHelper.GetListSetting<string>(SettingNames.BlockText)?.Any(item => string.IsNullOrEmpty(item) ? false : data.Text.Contains(item)) == true) ||
                    (data.ReplyComment?.Text != null && SettingHelper.GetListSetting<string>(SettingNames.BlockText)?.Any(item => string.IsNullOrEmpty(item) ? false : data.ReplyComment.Text.Contains(item)) == true) ?
                    Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}