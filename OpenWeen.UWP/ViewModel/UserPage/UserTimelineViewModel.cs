using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;

namespace OpenWeen.UWP.ViewModel.UserPage
{
    public class UserTimelineViewModel : UserViewModelBase<MessageModel>
    {
        public UserTimelineViewModel(long uid) : base(uid)
        {
        }

        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride()
        {
            var list = (await Core.Api.Statuses.UserTimeline.GetUserTimeline(Uid, count: LoadCount, max_id: WeiboList[WeiboList.Count - 1].ID)).Statuses;
            list.RemoveAt(0);
            return list;
        }

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            var item = await Core.Api.Statuses.UserTimeline.GetUserTimeline(Uid, count: LoadCount);
            return Tuple.Create(item.TotalNumber, item.Statuses);
        }
    }
}