using OpenWeen.Core.Helper;
using OpenWeen.Core.Model.Comment;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Controls;
using OpenWeen.UWP.Common.Controls.Events;
using OpenWeen.UWP.Common.Entities;
using OpenWeen.UWP.Common.Helpers;
using OpenWeen.UWP.View;
using OpenWeen.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace OpenWeen.UWP.Model
{
    public class WeiboActionModel
    {
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
                (Window.Current.Content as Frame).Navigate(typeof(PostWeiboPage), new ReplyCommentData(data.Status.ID, data.ID, $"回复@{data.User.ScreenName}:"));
            }
        }
        public void Repost(object sender, WeiboActionEventArgs e)
        {
            var item = e.TargetItem as MessageModel;
            (Window.Current.Content as Frame).Navigate(typeof(PostWeiboPage), new RepostData(item.ID, item.RetweetedStatus == null ? "" : $"//@{item.User.Name}:{item.Text}"));
        }

        private bool _isFavoring;
        public async void Favor(object sender, WeiboActionEventArgs e)
        {
            if (_isFavoring)
                return;
            _isFavoring = true;
            if (!(e.TargetItem is MessageModel))
                throw new ArgumentException("TargetItem must be MessageModel");
            var item = e.TargetItem as MessageModel;
            var list = sender as WeiboList;
            var favorIcon = MoreVisualTreeHelper.GetObjectByName<SymbolIcon>(list.ContainerFromItem(item), "FavorIcon");
            try
            {
                (list.ItemFromContainer(list.ContainerFromItem(item)) as MessageModel).Favorited = await FavorAndChangeSymbolIcon(item, favorIcon);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is WebException)
            {

            }
            _isFavoring = false;
        }

        internal async Task<bool> FavorAndChangeSymbolIcon(MessageModel item, SymbolIcon favorIcon)
        {
            var state = item.Favorited ?
                (await Core.Api.Favorites.RemoveFavor(item.ID)).Status.Favorited :
                (await Core.Api.Favorites.AddFavor(item.ID)).Status.Favorited;
            favorIcon.Foreground = state ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.LightGray);
            return state;
        }

        public async void PictureClick(object sender, WeiboPictureClickEventArgs e)
        {
            var items = e.DataContext.PicUrls.Select(item => new ImageModel((PictureHelper.GetOriginalPicFromID(PictureHelper.GetIDFromThumbnail(item.ThumbnailPic))))).ToList();
            var dialog = new ImageViewDialog(items);
            await dialog.ShowAsync();
        }

        public void ItemClick(object sender, WeiboItemClickEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(WeiboDetailPage), new WeiboDetailViewModel(e.ClickedItem));
        }

        public void TopicClick(object sender, WeiboTopicClickEventArgs e)
        {

        }
    }
}
