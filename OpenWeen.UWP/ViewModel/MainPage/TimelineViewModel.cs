using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Extension;


namespace OpenWeen.UWP.ViewModel.MainPage
{
    public class TimelineViewModel : WeiboListViewModelBase<MessageModel>
    {
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride()
        {
            var list = (await Core.Api.Statuses.Home.GetTimeline(max_id: WeiboList[WeiboList.Count - 1].ID)).Statuses;
            list.RemoveAt(0);
            return list;
        }

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            var item = await Core.Api.Statuses.Home.GetTimeline();
            return Tuple.Create(item.TotalNumber, item.Statuses);
        }
    }
}