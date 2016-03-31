using System.ComponentModel;
using OpenWeen.UWP.Common.Entities;

namespace OpenWeen.UWP.Model
{
    public class FollowModel : INotifyPropertyChanged
    {
        public FollowState State { get; private set; } = FollowState.None;

        public string Text
        {
            get
            {
                switch (State)
                {
                    case FollowState.None:
                        return "关注";

                    case FollowState.Following:
                        return "正在关注";

                    case FollowState.Followed:
                        return "被关注";

                    case FollowState.FollowBoth:
                        return "互相关注";

                    case FollowState.Bloking:
                        return "黑名单中";

                    default:
                        return "";
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetState(bool isFollowing, bool isFollowed, bool isBlocked)
        {
            State = isBlocked ? FollowState.Bloking : isFollowed && isFollowing ? FollowState.FollowBoth : isFollowing ? FollowState.Following : isFollowed ? FollowState.Followed : FollowState.None;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
        }
    }
}