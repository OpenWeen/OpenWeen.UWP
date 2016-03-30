using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
{
    public class UserTimelineViewModel : UserViewModelBase<MessageModel>
    {
        public UserTimelineViewModel(long uid) : base(uid)
        {
        }
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride() => (await Core.Api.Statuses.UserTimeline.GetUserTimeline(Uid, page: _pageCount++)).Statuses;
        
        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            var item = await Core.Api.Statuses.UserTimeline.GetUserTimeline(Uid, page: _pageCount++);
            return Tuple.Create(item.TotalNumber, item.Statuses);
        }
    }
}
