using System;
using OpenWeen.Core.Model;

namespace OpenWeen.UWP.Common.Controls.Events
{
    public class WeiboActionEventArgs : EventArgs
    {
        public WeiboActionEventArgs(BaseModel item)
        {
            TargetItem = item;
        }

        public BaseModel TargetItem { get; set; }
    }
}