using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Comment;

namespace OpenWeen.UWP.ViewModel.MainPage
{
    public class CommentViewModel : WeiboListViewModelBase<CommentModel>
    {
        protected override async Task<IEnumerable<CommentModel>> LoadMoreOverride()
        {
            var list = (await Core.Api.Comments.GetCommentToMe(max_id: WeiboList[WeiboList.Count - 1].ID, count: LoadCount)).Comments;
            list.RemoveAt(0);
            return list;
        }

        protected override async Task<Tuple<int, List<CommentModel>>> RefreshOverride()
        {
            await Core.Api.Remind.ClearUnRead(Core.Api.RemindType.Cmt);
            var item = await Core.Api.Comments.GetCommentToMe(count: LoadCount);
            return Tuple.Create(item.TotalNumber, item.Comments);
        }
    }
}