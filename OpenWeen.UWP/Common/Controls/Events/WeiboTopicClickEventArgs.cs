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