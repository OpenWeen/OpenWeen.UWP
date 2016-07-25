using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;

namespace OpenWeen.UWP.ViewModel.DetailPage
{
    public class WeiboRepostViewModel : WeiboDetailListBase<MessageModel>
    {
        public WeiboRepostViewModel(long id) : base(id)
        {
        }

        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride()
        {
            var list = (await Core.Api.Statuses.Repost.GetRepost(ID, max_id: WeiboList[WeiboList.Count - 1].ID, count: LoadCount)).Reposts;
            list.RemoveAt(0);
            return list;
        }

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            var item = await Core.Api.Statuses.Repost.GetRepost(ID, count: LoadCount);
            return Tuple.Create(item.TotalNumber, item.Reposts);
        }
    }
}