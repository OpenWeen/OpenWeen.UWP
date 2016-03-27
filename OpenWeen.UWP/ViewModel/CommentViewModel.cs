using OpenWeen.Core.Model.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
{
    public class CommentViewModel : WeiboListViewModelBase<CommentModel>
    {
        protected override async Task<IEnumerable<CommentModel>> LoadMoreOverride() => (await Core.Api.Comments.GetComment(page: _pageCount++)).Comments;

        protected override async Task<IEnumerable<CommentModel>> RefreshOverride()
        {
            await Core.Api.Remind.ClearUnRead(Core.Api.RemindType.Cmt);
            return (await Core.Api.Comments.GetComment(page: _pageCount++)).Comments;
        }
    }
}
