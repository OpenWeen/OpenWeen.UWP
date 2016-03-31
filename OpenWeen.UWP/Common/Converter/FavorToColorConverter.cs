using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace OpenWeen.UWP.Common.Converter
{
    public class FavorToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => (bool)value ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.LightGray);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}