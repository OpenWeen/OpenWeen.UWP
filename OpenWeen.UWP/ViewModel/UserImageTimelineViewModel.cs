using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
{
    public class UserImageTimelineViewModel : UserViewModelBase<MessageModel>
    {
        public UserImageTimelineViewModel(long uid) : base(uid)
        {
        }
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride() 
            => (await Core.Api.Statuses.UserTimeline.GetUserTimeline(Uid, page: _pageCount++, feature: Core.Model.Types.FeatureType.Picture)).Statuses;

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            var item = await Core.Api.Statuses.UserTimeline.GetUserTimeline(Uid, page: _pageCount++, feature: Core.Model.Types.FeatureType.Picture);
            return Tuple.Create(item.TotalNumber, item.Statuses);
        }
    }
}
