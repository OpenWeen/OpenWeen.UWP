using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml.Data;

namespace OpenWeen.UWP.Common.Converter
{
    public class WeiboImageRelyOnNetwork : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            var item = value as PictureModel;
            var networkProfile = NetworkInformation.GetInternetConnectionProfile();
            if (networkProfile.IsWlanConnectionProfile || !networkProfile.IsWwanConnectionProfile)
                return item.ToBmiddle;
            return item.ThumbnailPic;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
