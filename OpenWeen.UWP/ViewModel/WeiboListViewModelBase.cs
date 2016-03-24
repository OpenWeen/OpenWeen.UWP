using OpenWeen.Core.Model.Status;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.ViewModel
{
    public abstract class WeiboListViewModelBase<T> : INotifyPropertyChanged
    {
        public ObservableCollection<T> WeiboList { get; private set; } = new ObservableCollection<T>();
        protected int _pageCount = 1;
        private bool _isLoading;

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task Refresh()
        {
            if (_isLoading)
                return;
            _isLoading = true;
            try
            {
                _pageCount = 1;
                WeiboList = new ObservableCollection<T>((await RefreshOverride()));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WeiboList)));
            }
            catch (Exception e) when (e is HttpRequestException || e is WebException)
            {
#if DEBUG
                throw;
#endif
                OnWebException();
            }
            _isLoading = false;
        }

        public async Task LoadMore()
        {
            if (_isLoading)
                return;
            _isLoading = true;
            try
            {
                (await LoadMoreOverride()).ToList().ForEach(item => WeiboList.Add(item));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WeiboList)));
            }
            catch (Exception e) when (e is HttpRequestException || e is WebException)
            {
#if DEBUG
                throw;
#endif
                OnWebException();
            }
            _isLoading = false;
        }

        internal async Task<bool> Favor(object sender, Common.Controls.Events.WeiboActionEventArgs e)
        {
            if (!(e.TargetItem is MessageModel))
                throw new ArgumentException("TargetItem must be MessageModel");
            var item = e.TargetItem as MessageModel;
            try
            {
                return item.Favorited ?
                    (await Core.Api.Favorites.RemoveFavor(item.ID)).Status.Favorited :
                    (await Core.Api.Favorites.AddFavor(item.ID)).Status.Favorited;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is WebException)
            {
                OnWebException();
                return item.Favorited;
            }
        }

        private void OnWebException()
        {
            throw new NotImplementedException();
        }

        protected abstract Task<IEnumerable<T>> LoadMoreOverride();

        protected abstract Task<IEnumerable<T>> RefreshOverride();
        
    }
}
