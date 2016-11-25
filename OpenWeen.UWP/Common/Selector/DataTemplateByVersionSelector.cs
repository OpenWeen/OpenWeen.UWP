using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenWeen.UWP.Common.Selector
{
    public class DataTemplateByVersionSelector : DataTemplateSelector
    {
        public DataTemplate BeforeAnniversary { get; set; }
        public DataTemplate AfterAnniversary { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Media.Imaging.BitmapImage", nameof(BitmapImage.IsAnimatedBitmap)))
                return AfterAnniversary;
            else
                return BeforeAnniversary;
        }
    }
}
