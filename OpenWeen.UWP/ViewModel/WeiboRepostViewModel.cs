using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;

namespace OpenWeen.UWP.ViewModel
{
    public class WeiboRepostViewModel : WeiboDetailListBase<MessageModel>
    {
        public WeiboRepostViewModel(long id) : base(id)
        {
        }

        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride() => (await Core.Api.Statuses.Repost.GetRepost(ID, page: _pageCount++)).Reposts;

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            var item = await Core.Api.Statuses.Repost.GetRepost(ID, page: _pageCount++);
            return Tuple.Create(item.TotalNumber, item.Reposts);
        }
    }
}