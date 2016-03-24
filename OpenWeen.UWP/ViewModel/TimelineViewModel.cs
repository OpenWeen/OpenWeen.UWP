using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OpenWeen.UWP.ViewModel
{
    public class TimelineViewModel : WeiboListViewModelBase<MessageModel>
    {
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride() => (await Core.Api.Statuses.Home.GetTimeline(page: _pageCount++)).Statuses;

        protected override async Task<IEnumerable<MessageModel>> RefreshOverride() => (await Core.Api.Statuses.Home.GetTimeline(page: _pageCount++)).Statuses;
    }
}
