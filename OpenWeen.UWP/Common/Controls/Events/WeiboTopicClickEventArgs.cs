using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.Common.Controls.Events
{
    public class WeiboTopicClickEventArgs
    {
        public WeiboTopicClickEventArgs(string topic)
        {
            Topic = topic;
        }
        public string Topic { get; set; }
    }
}
