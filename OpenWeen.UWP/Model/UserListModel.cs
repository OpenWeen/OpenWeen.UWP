using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using OpenWeen.Core.Model.User;

namespace OpenWeen.UWP.Model
{
    public class UserListModel
    {
        public string AccessToken { get; }
        public INotifyTaskCompletion<UserModel> User { get; }
        public UserListModel(string accessToken)
        {
            AccessToken = accessToken;
            User = NotifyTaskCompletion.Create(GetUser);
        }
        private async Task<UserModel> GetUser() => await Core.Api.User.User.GetUser(long.Parse(await Core.Api.User.Account.GetUid(AccessToken)));
    }
}
