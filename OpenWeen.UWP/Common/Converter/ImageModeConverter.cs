using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.UWP.Shared.Common;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OpenWeen.UWP.Common.Converter
{
    public class ImageModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (Settings.IsAutoOffImage && NetworkInformation.GetInternetConnectionProfile().IsWwanConnectionProfile)
                return Visibility.Collapsed;
            return Settings.IsOffImage ? Visibility.Collapsed: Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
