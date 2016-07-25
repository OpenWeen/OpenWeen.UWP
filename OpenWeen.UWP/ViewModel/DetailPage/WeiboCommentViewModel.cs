using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Comment;

namespace OpenWeen.UWP.ViewModel.DetailPage
{
    public class WeiboCommentViewModel : WeiboDetailListBase<CommentModel>
    {
        public WeiboCommentViewModel(long id) : base(id)
        {
        }

        protected override async Task<IEnumerable<CommentModel>> LoadMoreOverride()
        {
            var list = (await Core.Api.Comments.GetCommentStatus(ID, max_id: WeiboList[WeiboList.Count - 1].ID, count: LoadCount)).Comments;
            list.RemoveAt(0);
            return list;
        }

        protected override async Task<Tuple<int, List<CommentModel>>> RefreshOverride()
        {
            var item = await Core.Api.Comments.GetCommentStatus(ID, count: LoadCount);
            return Tuple.Create(item.TotalNumber, item.Comments);
        }
    }
}