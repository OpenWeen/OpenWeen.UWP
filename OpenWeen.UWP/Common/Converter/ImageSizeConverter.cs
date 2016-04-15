using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.UI.Xaml.Data;

namespace OpenWeen.UWP.Common.Converter
{
    public class ImageSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => Settings.ImageSize;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
