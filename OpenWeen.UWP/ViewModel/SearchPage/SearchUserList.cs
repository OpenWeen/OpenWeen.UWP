using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.User;

namespace OpenWeen.UWP.ViewModel.SearchPage
{
    public class SearchUserList : WeiboListViewModelBase<UserModel>
    {
        private string _text;
        internal void SetTextAndRefresh(string text)
        {
            _text = text;
            Refresh();
        }
        protected override async Task<IEnumerable<UserModel>> LoadMoreOverride() => (await Core.Api.Search.Search.SearchUsers(_text, page: _pageCount++, count: LoadCount)).Users;

        protected override async Task<Tuple<int, List<UserModel>>> RefreshOverride()
        {
            if (string.IsNullOrEmpty(_text))
                return null;
            return Tuple.Create(int.MaxValue, (await Core.Api.Search.Search.SearchUsers(_text, page: _pageCount++, count: LoadCount)).Users);//TODO:TotalNumber is wrong
        }
    }
}
