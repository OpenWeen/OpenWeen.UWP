using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace OpenWeen.UWP.ViewModel.SearchPage
{
    public class SearchPageViewModel
    {
        public SearchWeiboList WeiboList { get; } = new SearchWeiboList();
        public SearchUserList UserList { get; } = new SearchUserList();
        public string SearchText { get; set; }

        internal void Search(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (string.IsNullOrEmpty(SearchText))
                return;
            WeiboList.SetTextAndRefresh(SearchText);
            UserList.SetTextAndRefresh(SearchText);
        }
    }
}
