using System;
using System.ComponentModel;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Controls;
using Nito.AsyncEx;
using System.Threading.Tasks;
using PropertyChanged;

namespace OpenWeen.UWP.ViewModel.DetailPage
{
    [ImplementPropertyChanged]
    public class WeiboDetailViewModel
    {
        public INotifyTaskCompletion<MessageModel> Item { get; }
        public WeiboRepostViewModel Repost { get; }
        public WeiboCommentViewModel Comment { get; }
        public string SendText { get; set; }

        public WeiboDetailViewModel(MessageModel item)
        {
            Item = NotifyTaskCompletion.Create(Init(item));
            Repost = new WeiboRepostViewModel(item.ID);
            Comment = new WeiboCommentViewModel(item.ID);
            Repost.Refresh();
            Comment.Refresh();
        }

        private async Task<MessageModel> Init(MessageModel item)
        {
            if (item.Text.IndexOf("全文： http://m.weibo.cn/") != -1)
            {
                var status = await Core.Api.Statuses.Query.GetStatus(item.ID, true);
                status.Text = status.LongText.Content;
                //Item.UrlStruct = Item.LongText.UrlStruct;
                return status;
            }
            else
            {
                return item;
            }
        }
        public async void NewRepost()
        {
            var sit = new SitbackAndRelaxDialog();
            sit.ShowAsync();
            try
            {
                await Core.Api.Statuses.PostWeibo.Repost(Item.Result.ID, (Item.Result.RetweetedStatus == null ? $"{SendText}" : $"{SendText}//@{Item.Result.User.Name}:{Item.Result.Text}").Remove(139));
            }
            catch { }
            sit.Hide();
            SendText = "";
        }
        public async void NewComment()
        {
            if (string.IsNullOrEmpty(SendText))
                return;
            var sit = new SitbackAndRelaxDialog();
            sit.ShowAsync();
            try
            {
                await Core.Api.Comments.PostComment(Item.Result.ID, SendText.Remove(139));
            }
            catch { }
            sit.Hide();
            SendText = "";
        }
    }
}