using OpenWeen.Core.Model;

namespace OpenWeen.UWP.ViewModel.DetailPage
{
    public abstract class WeiboDetailListBase<T> : WeiboListViewModelBase<T> where T : BaseModel
    {
        public WeiboDetailListBase(long id)
        {
            ID = id;
        }

        public long ID { get; }
    }
}