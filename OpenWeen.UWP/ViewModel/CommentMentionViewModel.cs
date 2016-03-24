using OpenWeen.Core.Model.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
{
    public class CommentMentionViewModel : WeiboListViewModelBase<CommentModel>
    {
        protected override async Task<IEnumerable<CommentModel>> LoadMoreOverride() => (await Core.Api.Comments.GetCommentMentions(page: _pageCount++)).Comments;

        protected override async Task<IEnumerable<CommentModel>> RefreshOverride() => (await Core.Api.Comments.GetCommentMentions(page: _pageCount++)).Comments;
    }
}
