using System;
using System.ComponentModel;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Controls;
using Nito.AsyncEx;
using System.Threading.Tasks;
using PropertyChanged;
using System.Linq;

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
            if (item.Text.Contains("全文： http://m.weibo.cn/") || item.Text.Contains("http://m.weibo.cn/client/version"))
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
                var text = Item.Result.RetweetedStatus == null ? $"{SendText}" : $"{SendText}//@{Item.Result.User.Name}:{Item.Result.Text}";
                await Core.Api.Statuses.PostWeibo.Repost(Item.Result.ID, text.Length > 140 ? text.Remove(139) : text);
            }
            catch { Notification.Show("发送失败"); }
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
                await Core.Api.Comments.PostComment(Item.Result.ID, SendText.Length > 140 ? SendText.Remove(139) : SendText);
            }
            catch { Notification.Show("发送失败"); }
            sit.Hide();
            SendText = "";
        }
    }
}