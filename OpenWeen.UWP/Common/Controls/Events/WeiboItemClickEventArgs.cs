using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.Common.Controls.Events
{
    public class WeiboItemClickEventArgs : EventArgs
    {
        public WeiboItemClickEventArgs(MessageModel clickedItem)
        {
            ClickedItem = clickedItem;
        }
        public MessageModel ClickedItem { get; set; }
    }
}
