using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.UWP.Shared.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenWeen.UWP.Common.Selector
{
    public class WeiboListSelector : DataTemplateSelector
    {
        public DataTemplate WeiboListView { get; set; }
        public DataTemplate WaterFallView { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => Settings.EnableWaterFall ? WaterFallView : WeiboListView;
    }
}
