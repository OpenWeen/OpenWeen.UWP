using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.DirectMessage;

namespace OpenWeen.UWP.ViewModel.MainPage
{
    public class MessageUserListViewModel : WeiboListViewModelBase<DirectMessageUserModel>
    {
        protected override async Task<IEnumerable<DirectMessageUserModel>> LoadMoreOverride()
        {
            var item = await Core.Api.DirectMessages.GetUserList(cursor: _pageCount, count: LoadCount);
            _pageCount = int.Parse(item.NextCursor);
            return item.UserList;
        }

        protected override async Task<Tuple<int, List<DirectMessageUserModel>>> RefreshOverride()
        {
            _pageCount = 0;
            await Core.Api.Remind.ClearUnRead(Core.Api.RemindType.Dm);
            var item = await Core.Api.DirectMessages.GetUserList(cursor: _pageCount, count: LoadCount);
            _pageCount = int.Parse(item.NextCursor);
            return Tuple.Create(item.TotalNumber, item.UserList);
        }
    }
}
