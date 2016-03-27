using OpenWeen.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenWeen.UWP.Common.Converter
{
    public class PicIDToThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => new BitmapImage(new Uri(PictureHelper.GetThumbnailPicFromID(value.ToString())));

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
