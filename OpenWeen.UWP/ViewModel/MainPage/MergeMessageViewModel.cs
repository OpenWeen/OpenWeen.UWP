using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model;

namespace OpenWeen.UWP.ViewModel.MainPage
{
    public class MergeMessageViewModel : WeiboListViewModelBase<BaseModel>
    {
        protected override async Task<IEnumerable<BaseModel>> LoadMoreOverride()
        {
            var commentMentions = (await Core.Api.Comments.GetCommentMentions(max_id: WeiboList[WeiboList.Count - 1].ID, count: LoadCount)).Comments;
            commentMentions.RemoveAt(0);
            var comments = (await Core.Api.Comments.GetCommentToMe(max_id: WeiboList[WeiboList.Count - 1].ID, count: LoadCount)).Comments;
            comments.RemoveAt(0);
            var mentions = (await Core.Api.Statuses.Mentions.GetMentions(max_id: WeiboList[WeiboList.Count - 1].ID, count: LoadCount)).Statuses;
            mentions.RemoveAt(0);
            return mentions.Concat<BaseModel>(comments).Concat(commentMentions).OrderByDescending(item => item.CreateTime);
        }

        protected override async Task<Tuple<int, List<BaseModel>>> RefreshOverride()
        {
            Core.Api.Remind.ClearUnRead(Core.Api.RemindType.MentionStatus);
            var mentions = await Core.Api.Statuses.Mentions.GetMentions(count: LoadCount);
            Core.Api.Remind.ClearUnRead(Core.Api.RemindType.Cmt);
            var comments = await Core.Api.Comments.GetCommentToMe(count: LoadCount);
            Core.Api.Remind.ClearUnRead(Core.Api.RemindType.MentionCmt);
            var commentMentions = await Core.Api.Comments.GetCommentMentions(count: LoadCount);
            return Tuple.Create(mentions.TotalNumber + comments.TotalNumber + commentMentions.TotalNumber, mentions.Statuses.Concat<BaseModel>(comments.Comments).Concat(commentMentions.Comments).OrderByDescending(item=>item.CreateTime).ToList());
        }
    }
}
