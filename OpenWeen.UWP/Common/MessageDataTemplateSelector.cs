using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.DirectMessage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenWeen.UWP.Common
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MessageFromMe { get; set; }
        public DataTemplate MessageFromOther { get; set; }
        
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var model = item as DirectMessageModel;
            return model.SenderID == StaticResource.Uid ? MessageFromMe : MessageFromOther;
        }
    }
}
