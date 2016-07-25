using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Comment;

namespace OpenWeen.UWP.ViewModel.MainPage
{
    public class CommentMentionViewModel : WeiboListViewModelBase<CommentModel>
    {
        protected override async Task<IEnumerable<CommentModel>> LoadMoreOverride()
        {
            var list = (await Core.Api.Comments.GetCommentMentions(max_id: WeiboList[WeiboList.Count - 1].ID, count: LoadCount)).Comments;
            list.RemoveAt(0);
            return list;
        }

        protected override async Task<Tuple<int, List<CommentModel>>> RefreshOverride()
        {
            await Core.Api.Remind.ClearUnRead(Core.Api.RemindType.MentionCmt);
            var item = await Core.Api.Comments.GetCommentMentions(count: LoadCount);
            return Tuple.Create(item.TotalNumber, item.Comments);
        }
    }
}