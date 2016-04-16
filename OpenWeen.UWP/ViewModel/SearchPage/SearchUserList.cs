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
        protected override Task<IEnumerable<UserModel>> LoadMoreOverride()
        {
            throw new NotImplementedException();
        }

        protected override async Task<Tuple<int, List<UserModel>>> RefreshOverride()
        {
            throw new NotImplementedException();
            //if (string.IsNullOrEmpty(_text))
            //    return null;
            //var item = await Core.Api.Search.Search.SearchUsers(_text, page: _pageCount++);
            //return Tuple.Create(int.MaxValue,item);//TODO:TotalNumber is wrong
        }
    }
}
