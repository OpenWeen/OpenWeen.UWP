using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;

namespace OpenWeen.UWP.ViewModel.SearchPage
{
    public class SearchWeiboList : WeiboListViewModelBase<MessageModel>
    {
        private string _text;
        internal void SetTextAndRefresh(string text)
        {
            _text = text;
            Refresh();
        }
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride() => (await Core.Api.Search.Search.SearchStatus(_text, page: _pageCount++)).Statuses;

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            if (string.IsNullOrEmpty(_text))
                return null;
            var item = await Core.Api.Search.Search.SearchStatus(_text, page: _pageCount++);
            return Tuple.Create(item.TotalNumber, item.Statuses);//TODO:TotalNumber is wrong
        }
    }
}
