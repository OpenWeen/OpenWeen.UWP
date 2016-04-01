using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.User;

namespace OpenWeen.UWP.ViewModel
{
    public abstract class UserListViewModelBase : WeiboListViewModelBase<UserModel>
    {
        public long UID { get; private set; }
        public string Header { get; private set; }
        protected UserListViewModelBase(long uid, string header)
        {
            UID = uid;
            Header = header;
            Refresh();
        }
    }
}
