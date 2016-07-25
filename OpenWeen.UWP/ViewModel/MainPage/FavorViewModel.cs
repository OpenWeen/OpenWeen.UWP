using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Favor;
using OpenWeen.Core.Model.Status;

namespace OpenWeen.UWP.ViewModel.MainPage
{
    public class FavorViewModel : WeiboListViewModelBase<MessageModel>
    {
        protected override async Task<IEnumerable<MessageModel>> LoadMoreOverride() => (await Core.Api.Favorites.GetFavorList(page: _pageCount++, count: LoadCount)).Favorites.Select(item => item.Status);


        protected override async Task<Tuple<int, List<MessageModel>>> RefreshOverride()
        {
            var item = await Core.Api.Favorites.GetFavorList(page: _pageCount++, count: LoadCount);
            return Tuple.Create(item.TotalNumber, item.Favorites.Select(favor => favor.Status).ToList());
        }
    }
}
