using OpenWeen.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
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
