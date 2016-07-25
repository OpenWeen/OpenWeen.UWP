using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;

namespace OpenWeen.UWP.ViewModel.UserPage
{
    public class UserImageTimelineViewModel : UserViewModelBase<MessageModel>
    {
        public UserImageTimelineViewModel(long uid) : base(uid)
        {
        }

        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride()
            => (await Core.Api.Statuses.UserTimeline.GetUserTimeline(Uid, count: LoadCount, page: _pageCount++, feature: Core.Model.Types.FeatureType.Picture)).Statuses;

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            var item = await Core.Api.Statuses.UserTimeline.GetUserTimeline(Uid, count: LoadCount, page: _pageCount++, feature: Core.Model.Types.FeatureType.Picture);
            return Tuple.Create(item.TotalNumber, item.Statuses);
        }
    }
}