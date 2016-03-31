using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;

namespace OpenWeen.UWP.ViewModel
{
    public class MentionViewModel : WeiboListViewModelBase<MessageModel>
    {
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride() => (await Core.Api.Statuses.Mentions.GetMentions(page: _pageCount++)).Statuses;

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            await Core.Api.Remind.ClearUnRead(Core.Api.RemindType.MentionStatus);
            var item = await Core.Api.Statuses.Mentions.GetMentions(page: _pageCount++);
            return Tuple.Create(item.TotalNumber, item.Statuses);
        }
    }
}