using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Comment;

namespace OpenWeen.UWP.ViewModel
{
    public class CommentMentionViewModel : WeiboListViewModelBase<CommentModel>
    {
        protected override async Task<IEnumerable<CommentModel>> LoadMoreOverride() => (await Core.Api.Comments.GetCommentMentions(page: _pageCount++)).Comments;

        protected override async Task<Tuple<int, List<CommentModel>>> RefreshOverride()
        {
            await Core.Api.Remind.ClearUnRead(Core.Api.RemindType.MentionCmt);
            var item = await Core.Api.Comments.GetCommentMentions(page: _pageCount++);
            return Tuple.Create(item.TotalNumber, item.Comments);
        }
    }
}