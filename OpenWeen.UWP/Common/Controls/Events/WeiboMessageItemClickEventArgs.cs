using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.DirectMessage;

namespace OpenWeen.UWP.Common.Controls.Events
{
    public class WeiboMessageItemClickEventArgs : EventArgs
    {
        public DirectMessageUserModel ClickedItem { get; }
        public WeiboMessageItemClickEventArgs(DirectMessageUserModel clickedItem)
        {
            ClickedItem = clickedItem;
        }
    }
}
