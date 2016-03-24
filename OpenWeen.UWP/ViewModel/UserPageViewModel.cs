using OpenWeen.Core.Model.User;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
{
    public class UserPageViewModel : INotifyPropertyChanged
    {
        public UserTimelineViewModel UserTimeline { get; private set; }
        public UserImageTimelineViewModel UserImageTimeline { get; private set; }
        public UserModel User { get; private set; }
        public FollowModel Follow { get; } = new FollowModel();
        public dynamic UidOrUserName { get; }
        public bool IsBlocked { get; private set; }
        public bool IsMe => StaticResource.Uid == User?.ID;

        public UserPageViewModel(long uid)
        {
            UidOrUserName = uid;
            Init();
        }
        public UserPageViewModel(string userName)
        {
            UidOrUserName = userName;
            Init();
        }
        
        public UserPageViewModel(object uidOrUserName)
        {
            if (!(uidOrUserName is string || uidOrUserName is long))
                throw new ArgumentException("parameter must be string or long");
            UidOrUserName = uidOrUserName;
            Init();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Init()
        {
            UserTimeline = new UserTimelineViewModel(UidOrUserName);
            UserTimeline.Refresh();
            UserImageTimeline = new UserImageTimelineViewModel(UidOrUserName);
            UserImageTimeline.Refresh();
            User = await Core.Api.User.User.GetUser(UidOrUserName);
            //Fuck weibo does not weico to check the block state
            //IsBlocked = await Core.Api.Blocks.IsBlocked(User.ID);
            Follow.SetState(User.Following, User.FollowMe, IsBlocked);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMe)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserTimeline)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserImageTimeline)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(User)));
        }

        public async void AddBlock()
        {
            await Core.Api.Blocks.AddBlock(User.ID);
        }
    }
}
