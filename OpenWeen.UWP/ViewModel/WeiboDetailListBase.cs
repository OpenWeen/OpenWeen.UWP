using OpenWeen.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
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
