using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OpenWeen.Core.Helper;
using OpenWeen.Core.Model.Comment;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Controls;
using OpenWeen.UWP.Common.Controls.Events;
using OpenWeen.UWP.Common.Entities;
using OpenWeen.UWP.Common.Helpers;
using OpenWeen.UWP.View;
using OpenWeen.UWP.ViewModel;
using OpenWeen.UWP.ViewModel.DetailPage;
using OpenWeen.UWP.ViewModel.MessagePage;
using OpenWeen.UWP.ViewModel.UserPage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace OpenWeen.UWP.Model
{
    public class WeiboActionModel
    {
        public static WeiboActionModel Instance { get; } = new WeiboActionModel();
        private WeiboActionModel()
        {

        }
        public void UserClick(object sender, WeiboUserClickEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(UserPage), new UserPageViewModel(e.UidOrUserName));
        }

        public void Comment(object sender, WeiboActionEventArgs e)
        {
            if (e.TargetItem is MessageModel)
            {
                var data = e.TargetItem as MessageModel;
                (Window.Current.Content as Frame).Navigate(typeof(PostWeiboPage), new CommentData(data.ID));
            }
            else if (e.TargetItem is CommentModel)
            {
                var data = e.TargetItem as CommentModel;
                (Window.Current.Content as Frame).Navigate(typeof(PostWeiboPage), new ReplyCommentData(data.Status.ID, data.ID, $"回复@{data.User.Name}:"));
            }
        }

        public void Repost(object sender, WeiboActionEventArgs e)
        {
            var item = e.TargetItem as MessageModel;
            if (item != null)
                (Window.Current.Content as Frame).Navigate(typeof(PostWeiboPage), new RepostData(item.ID, item.RetweetedStatus == null ? "" : $"//@{item.User.Name}:{item.Text}"));
        }
        
        public async void Favor(object sender, WeiboActionEventArgs e)
        {
            if (!(e.TargetItem is MessageModel))
                throw new ArgumentException("TargetItem must be MessageModel");
            var item = e.TargetItem as MessageModel;
            try
            {
                var state = item.Favorited ?
                    (await Core.Api.Favorites.RemoveFavor(item.ID)).Status.Favorited :
                    (await Core.Api.Favorites.AddFavor(item.ID)).Status.Favorited;
                Notification.Show(state ? "收藏成功" : "取消收藏成功");
            }
            catch
            {
                Notification.Show("收藏失败");
            }
        }

        public async void PictureClick(object sender, WeiboPictureClickEventArgs e)
        {
            var items = e.DataContext.PicUrls.Select(item => new ImageModel(item.OriginalPic == null ? PictureHelper.ThumbnailToOriginal(item.ThumbnailPic) : item.OriginalPic)).ToList();
            var dialog = new ImageViewDialog(items, e.DataContext.PicUrls.FindIndex((item=>item.ThumbnailPic == e.ClickedPicture.ThumbnailPic)));
            await dialog.ShowAsync();
        }

        public void ItemClick(object sender, WeiboItemClickEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(WeiboDetailPage), new WeiboDetailViewModel(e.ClickedItem));
        }

        public void TopicClick(object sender, WeiboTopicClickEventArgs e)
        {
        }

        public void MessageClick(object sender, WeiboMessageItemClickEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(MessagePage), new MessagePageViewModel(e.ClickedItem.User.ID, e.ClickedItem.User.ScreenName));
        }
        public async void Delete(object sender, WeiboActionEventArgs e)
        {
            try
            {
                await Core.Api.Statuses.PostWeibo.DeletePost(e.TargetItem.ID);
                Notification.Show("删除成功");

            }
            catch
            {
                Notification.Show("删除失败");
            }
        }
        public async void Like(object sender, WeiboActionEventArgs e)
        {
            try
            {
                await Core.Api.Attitudes.Like(e.TargetItem.ID);
                Notification.Show("点赞成功");
            }
            catch
            {
                Notification.Show("点赞失败");
            }
        }
    }
}