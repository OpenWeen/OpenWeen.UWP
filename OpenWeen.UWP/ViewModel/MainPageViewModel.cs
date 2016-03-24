using OpenWeen.Core.Model.Comment;
using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
{
    public class MainPageViewModel
    {
        public TimelineViewModel Timeline { get; } = new TimelineViewModel();
        public MentionViewModel Mention { get; } = new MentionViewModel();
        public CommentViewModel Comment { get; } = new CommentViewModel();
        public CommentMentionViewModel CommentMention { get; } = new CommentMentionViewModel();
        public MainPageViewModel()
        {
            Init();
        }

        private async void Init()
        {
            await Timeline.Refresh();
            await Mention.Refresh();
            await Comment.Refresh();
            //await CommentMention.Refresh();
        }
    }
}
