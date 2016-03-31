using System;
using OpenWeen.Core.Model.Status;

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