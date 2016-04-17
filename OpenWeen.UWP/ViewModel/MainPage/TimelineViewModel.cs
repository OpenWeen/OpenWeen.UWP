using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Extension;


namespace OpenWeen.UWP.ViewModel.MainPage
{
    public class TimelineViewModel : WeiboListViewModelBase<MessageModel>
    {
        private long _groupID = -1;
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride()
        {

            if (_groupID == -1)
            {
                var list = (await Core.Api.Statuses.Home.GetTimeline(max_id: WeiboList[WeiboList.Count - 1].ID)).Statuses;
                list.RemoveAt(0);
                return list;
            }
            else
            {
                var list = (await Core.Api.Friendships.Groups.GetGroupTimeline(_groupID.ToString(), max_id: WeiboList[WeiboList.Count - 1].ID)).Statuses;
                list.RemoveAt(0);
                return list;
            }
        }

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            if (_groupID == -1)
            {
                var item = await Core.Api.Statuses.Home.GetTimeline();
                return Tuple.Create(item.TotalNumber, item.Statuses);
            }
            else
            {
                var item = await Core.Api.Friendships.Groups.GetGroupTimeline(_groupID.ToString());
                return Tuple.Create(item.TotalNumber, item.Statuses);
            }
        }

        internal void SetGroupAndRefresh(GroupModel groupModel)
        {
            _groupID = groupModel.ID;
            Refresh();
        }
    }
}