using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OpenWeen.Core.Model;
using Windows.UI.Popups;
namespace OpenWeen.UWP.ViewModel
{
    public abstract class WeiboListViewModelBase<T> : INotifyPropertyChanged 
    {
        public ObservableCollection<T> WeiboList { get; private set; } = new ObservableCollection<T>();
        protected int _pageCount = 1;
        protected bool _hasMore => WeiboList.Count < _totalNumber;
        protected int _totalNumber = 0;

        protected bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public async Task Refresh()
        {
            if (IsLoading)
                return;
            IsLoading = true;
            try
            {
                _pageCount = 1;
                var item = await RefreshOverride();
                WeiboList = new ObservableCollection<T>((item.Item2));
                _totalNumber = item.Item1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WeiboList)));
            }
            catch (Exception e) when (e is HttpRequestException || e is WebException)
            {
#if DEBUG
                throw;
#endif
                OnWebException();
            }
            catch(Newtonsoft.Json.JsonException)
            {
                WeiboList = new ObservableCollection<T>();
            }
            IsLoading = false;
        }

        public async Task LoadMore()
        {
            if (IsLoading || !_hasMore)
                return;
            IsLoading = true;
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
            catch (Newtonsoft.Json.JsonException)
            {
                WeiboList = new ObservableCollection<T>();
            }
            IsLoading = false;
        }

        protected virtual async void OnWebException()
        {
            WeiboList = new ObservableCollection<T>();
        }

        protected abstract Task<IEnumerable<T>> LoadMoreOverride();

        protected abstract Task<Tuple<int, List<T>>> RefreshOverride();
    }
}