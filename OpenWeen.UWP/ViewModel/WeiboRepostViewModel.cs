using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
