using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.DirectMessage;
using WinRTXamlToolkit.Tools;

namespace OpenWeen.UWP.ViewModel.MessagePage
{
    public class MessagePageViewModel : WeiboListViewModelBase<DirectMessageModel>
    {
        private string _uid;
        private const int _count = 5;
        private BackgroundTimer _timer;

        public string UserName { get; }

        public string EnterText { get; set; }

        public MessagePageViewModel(long uid, string screenName)
        {
            UserName = screenName;
            _uid = uid.ToString();
            _timer = new BackgroundTimer() { Interval = TimeSpan.FromSeconds(10) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            Refresh();
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        }

        private void Timer_Tick(object sender, object e)
        {
            GetNew();
        }

        private async void GetNew()
        {
            long id = 0;
            lock (WeiboList)
                id = WeiboList[WeiboList.Count - 1].ID;
            var list = (await Core.Api.DirectMessages.GetConversation(_uid, since_id: id, count: _count));
            lock (WeiboList)
            {
                list.DirectMessages.ForEach(item => WeiboList.Add(item));
            }
        }

        internal void CleanUp()
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        protected override async Task<IEnumerable<DirectMessageModel>> LoadMoreOverride()
        {
            long id = 0;
            lock (WeiboList)
                id = WeiboList[0].ID;
            var list = (await Core.Api.DirectMessages.GetConversation(_uid, max_id: id, count: _count)).DirectMessages;
            list.RemoveAt(0);
            return list;
        }
        //can not use x:Bind to bind a override or new method
        public async void LoadMoreNew()
        {
            if (_isLoading || !_hasMore)
                return;
            _isLoading = true;
            try
            {
                var list = await LoadMoreOverride();
                lock (WeiboList)
                {
                    list.ToList().ForEach(item => WeiboList.Insert(0, item));
                }
            }
            catch (Exception e) when (e is HttpRequestException || e is WebException)
            {
#if DEBUG
                throw;
#endif
                OnWebException();
            }
            _isLoading = false;
        }

        public async void Send()
        {
            if (string.IsNullOrEmpty(EnterText))
                return;
            var text = EnterText;
            EnterText = "";
            OnPropertyChanged(nameof(EnterText));
            var item = await Core.Api.DirectMessages.Send(long.Parse(_uid), text);
            lock (WeiboList)
            {
                WeiboList.Add(item);
                OnPropertyChanged(nameof(WeiboList));
            }
        }

        protected override async Task<Tuple<int, List<DirectMessageModel>>> RefreshOverride()
        {
            var item = await Core.Api.DirectMessages.GetConversation(_uid, count: _count);
            item.DirectMessages.Reverse();
            return Tuple.Create(item.TotalNumber, item.DirectMessages);
        }

    }
}
