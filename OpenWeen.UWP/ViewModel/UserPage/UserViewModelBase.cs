using OpenWeen.Core.Model;

namespace OpenWeen.UWP.ViewModel.UserPage
{
    public abstract class UserViewModelBase<T> : WeiboListViewModelBase<T> where T : BaseModel
    {
        public long Uid { get; }

        protected UserViewModelBase()
        {
        }

        public UserViewModelBase(long uid)
        {
            Uid = uid;
        }
    }
}