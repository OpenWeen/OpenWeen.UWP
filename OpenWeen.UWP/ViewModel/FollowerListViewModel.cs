using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common;

namespace OpenWeen.UWP.ViewModel
{
    public class FollowerListViewModel : UserListViewModelBase
    {
        public FollowerListViewModel(long uid) : base(uid, "粉丝")
        {
        }

        protected override async Task<IEnumerable<UserModel>> LoadMoreOverride()
        {
            var item = await Core.Api.Friendships.Friends.GetFollowers(UID, cursor: _pageCount);
            _pageCount = int.Parse(item.NextCursor);
            return item.Users;
        }

        protected override async Task<Tuple<int, List<UserModel>>> RefreshOverride()
        {
            if (UID == StaticResource.Uid)
                await Core.Api.Remind.ClearUnRead(Core.Api.RemindType.Follower);
            var item = await Core.Api.Friendships.Friends.GetFollowers(UID, cursor: _pageCount);
            _pageCount = int.Parse(item.NextCursor);
            return Tuple.Create(item.TotalNumber, item.Users);
        }
    }
}
