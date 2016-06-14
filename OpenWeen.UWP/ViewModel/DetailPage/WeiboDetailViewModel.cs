using System;
using System.ComponentModel;
using OpenWeen.Core.Model.Status;

namespace OpenWeen.UWP.ViewModel.DetailPage
{
    public class WeiboDetailViewModel : INotifyPropertyChanged
    {
        public MessageModel Item { get; private set; }
        public WeiboRepostViewModel Repost { get; }
        public WeiboCommentViewModel Comment { get; }

        public bool IsLoading { get; private set; } = true;

        public WeiboDetailViewModel(MessageModel item)
        {
            Init(item);
            Repost = new WeiboRepostViewModel(item.ID);
            Comment = new WeiboCommentViewModel(item.ID);
            Repost.Refresh();
            Comment.Refresh();
        }

        private async void Init(MessageModel item)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item)));
            if (item.IsLongText)
            {
                try
                {
                    Item = await Core.Api.Statuses.Query.GetStatus(item.ID, true);
                    Item.Text = Item.LongText.Content;
                    Item.UrlStruct = Item.LongText.UrlStruct;
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Item = item;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item)));
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}