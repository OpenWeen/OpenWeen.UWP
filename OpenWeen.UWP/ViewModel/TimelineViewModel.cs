using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;

namespace OpenWeen.UWP.ViewModel
{
    public class TimelineViewModel : WeiboListViewModelBase<MessageModel>
    {
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride() => (await Core.Api.Statuses.Home.GetTimeline(page: _pageCount++)).Statuses;

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            var item = await Core.Api.Statuses.Home.GetTimeline(page: _pageCount++);
            return Tuple.Create(item.TotalNumber, item.Statuses);
        }
    }
}