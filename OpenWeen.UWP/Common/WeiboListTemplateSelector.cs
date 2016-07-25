using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Comment;
using OpenWeen.Core.Model.Status;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenWeen.UWP.Common
{
    public class WeiboListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommentTemplate { get; set; }
        public DataTemplate MessageTemplate { get; set; }
        public DataTemplate RepostListTemplate { get; set; }
        public bool IsRepostList { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (IsRepostList)
            {
                return RepostListTemplate;
            }
            else if (item is CommentModel)
            {
                return CommentTemplate;
            }
            else if (item is MessageModel)
            {
                return MessageTemplate;
            }
            return new DataTemplate();

        }
    }
}
