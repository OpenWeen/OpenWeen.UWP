using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OpenWeen.UWP.Common.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isInverted = false;

            if (parameter is string)
            {
                bool.TryParse(parameter.ToString(), out isInverted);
            }

            bool boolValue = (bool)value;

            boolValue = isInverted ? !boolValue : boolValue;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}