﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Comment;

namespace OpenWeen.UWP.ViewModel
{
    public class WeiboCommentViewModel : WeiboDetailListBase<CommentModel>
    {
        public WeiboCommentViewModel(long id) : base(id)
        {
        }

        protected override async Task<IEnumerable<CommentModel>> LoadMoreOverride() => (await Core.Api.Comments.GetCommentStatus(ID, page: _pageCount++)).Comments;

        protected override async Task<Tuple<int, List<CommentModel>>> RefreshOverride()
        {
            var item = await Core.Api.Comments.GetCommentStatus(ID, page: _pageCount++);
            return Tuple.Create(item.TotalNumber, item.Comments);
        }
    }
}