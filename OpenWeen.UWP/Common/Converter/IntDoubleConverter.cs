using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OpenWeen.UWP.Common.Converter
{
    public class IntDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => System.Convert.ToDouble(value);

        public object ConvertBack(object value, Type targetType, object parameter, string language) => System.Convert.ToInt32(value);
    }
}
