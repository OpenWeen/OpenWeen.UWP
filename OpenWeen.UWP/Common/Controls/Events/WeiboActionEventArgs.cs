using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
