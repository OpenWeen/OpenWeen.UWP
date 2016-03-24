using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
{
    public abstract class UserViewModelBase<T> : WeiboListViewModelBase<T>
    {
        public dynamic UidOrUserName { get; }
        protected UserViewModelBase()
        {

        }
        public UserViewModelBase(long uid)
        {
            UidOrUserName = uid;
        }
        public UserViewModelBase(string userName)
        {
            UidOrUserName = userName;
        }
        public UserViewModelBase(dynamic uidOrUserName)
        {
            if (!(uidOrUserName is string || uidOrUserName is long))
                throw new ArgumentException("parameter must be string or long");
            UidOrUserName = uidOrUserName;
        }
    }
}
