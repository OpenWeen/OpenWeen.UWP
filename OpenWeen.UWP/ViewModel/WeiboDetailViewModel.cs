using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
{
    public class WeiboDetailViewModel
    {
        public MessageModel Item { get; }
        public WeiboRepostViewModel Repost { get; }
        public WeiboCommentViewModel Comment { get; }
        public WeiboDetailViewModel(MessageModel item)
        {
            Item = item;
            Repost = new WeiboRepostViewModel(item.ID);
            Comment = new WeiboCommentViewModel(item.ID);
            Repost.Refresh();
            Comment.Refresh();
        }
    }
}
