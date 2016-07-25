using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;

namespace OpenWeen.UWP.ViewModel.MainPage
{
    public class MentionViewModel : WeiboListViewModelBase<MessageModel>
    {
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride()
        {
            var list = (await Core.Api.Statuses.Mentions.GetMentions(max_id: WeiboList[WeiboList.Count - 1].ID, count: LoadCount)).Statuses;
            list.RemoveAt(0);
            return list;
        }

        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            await Core.Api.Remind.ClearUnRead(Core.Api.RemindType.MentionStatus);
            var item = await Core.Api.Statuses.Mentions.GetMentions(count: LoadCount);
            return Tuple.Create(item.TotalNumber, item.Statuses);
        }
    }
}