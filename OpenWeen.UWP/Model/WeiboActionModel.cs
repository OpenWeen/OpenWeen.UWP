using OpenWeen.Core.Model.Comment;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Entities;
using OpenWeen.UWP.View;
using OpenWeen.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenWeen.UWP.Model
{
    public class WeiboActionModel
    {
        public void UserClick(object sender, Common.Controls.Events.WeiboUserClickEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(UserPage), new UserPageViewModel(e.UidOrUserName));
        }
        public void Comment(object sender, Common.Controls.Events.WeiboActionEventArgs e)
        {
            if (e.TargetItem is MessageModel)
            {
                var data = e.TargetItem as MessageModel;
                (Window.Current.Content as Frame).Navigate(typeof(PostWeiboPage), new CommentData(data.ID));
            }
            else if (e.TargetItem is CommentModel)
            {
                var data = e.TargetItem as CommentModel;
                (Window.Current.Content as Frame).Navigate(typeof(PostWeiboPage), new ReplyCommentData(data.Status.ID, data.ID, $"回复@{data.User.ScreenName}:"));
            }
        }
        public void Repost(object sender, Common.Controls.Events.WeiboActionEventArgs e)
        {
            var item = e.TargetItem as MessageModel;
            (Window.Current.Content as Frame).Navigate(typeof(PostWeiboPage), new RepostData(item.ID, item.Text));
        }

    }
}
