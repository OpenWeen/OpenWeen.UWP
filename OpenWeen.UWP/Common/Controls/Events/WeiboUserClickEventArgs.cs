using System;

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