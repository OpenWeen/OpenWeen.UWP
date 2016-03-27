using OpenWeen.Core.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.Common.Controls.Events
{
    public class WeiboUserClickEventArgs : EventArgs
    {
        public object UidOrUserName { get; }
        
        public WeiboUserClickEventArgs(string userName)
        {
            UidOrUserName = userName;
        }
        public WeiboUserClickEventArgs(long uid)
        {
            UidOrUserName = uid;
        }
    }
}
