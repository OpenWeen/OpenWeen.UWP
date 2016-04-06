using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.User;

namespace OpenWeen.UWP.ViewModel.UserPage
{
    public abstract class UserListViewModelBase : WeiboListViewModelBase<UserModel>
    {
        public long UID { get; }
        public string Header { get; }
        protected UserListViewModelBase(long uid, string header)
        {
            UID = uid;
            Header = header;
            Refresh();
        }
    }
}
