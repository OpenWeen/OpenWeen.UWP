using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
{
    public class UserTimelineViewModel : UserViewModelBase<MessageModel>
    {
        public UserTimelineViewModel(long uid) : base(uid)
        {
        }
        public UserTimelineViewModel(string userName) : base(userName)
        {
        }
        public UserTimelineViewModel(object uidOrUserName) : base(uidOrUserName)
        {
        }
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride() => (await Core.Api.Statuses.UserTimeline.GetUserTimeline(UidOrUserName, page: _pageCount++)).Statuses;
        protected override async Task<IEnumerable<MessageModel>> RefreshOverride() => (await Core.Api.Statuses.UserTimeline.GetUserTimeline(UidOrUserName, page: _pageCount++)).Statuses;
    }
}
