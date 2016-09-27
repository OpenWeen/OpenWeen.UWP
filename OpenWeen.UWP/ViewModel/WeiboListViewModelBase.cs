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
using OpenWeen.UWP.Common.Controls;
using OpenWeen.UWP.Shared.Common;
using Windows.UI.Popups;
namespace OpenWeen.UWP.ViewModel
{
    public abstract class WeiboListViewModelBase<T> : INotifyPropertyChanged 
    {
        protected int LoadCount => Settings.LoadCount;
        public ObservableCollection<T> WeiboList { get; private set; } = new ObservableCollection<T>();
        protected int _pageCount = 1;
        protected bool _hasMore
        {
            get
            {
                lock (WeiboList)
                    return WeiboList.Count < _totalNumber;
            }
        }
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
            lock (WeiboList)
            {
                WeiboList = new ObservableCollection<T>();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WeiboList)));
            }
            try
            {
                _pageCount = 1;
                var item = await RefreshOverride();
                lock (WeiboList)
                {
                    WeiboList = new ObservableCollection<T>((item.Item2));
                    _totalNumber = item.Item1;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WeiboList)));
                }
            }
            catch (Exception e) when (e is HttpRequestException || e is WebException || e is Newtonsoft.Json.JsonException || e is TaskCanceledException || e is ArgumentNullException)
            {
                OnWebException(e);
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
                var list = await LoadMoreOverride();
                lock (WeiboList)
                {
                    list.ToList().ForEach(item => WeiboList.Add(item));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WeiboList)));
                }
            }
            catch (Exception e) when (e is HttpRequestException || e is WebException || e is Newtonsoft.Json.JsonException || e is TaskCanceledException || e is ArgumentNullException)
            {
                _pageCount--;
            }
            IsLoading = false;
        }

        protected virtual void OnWebException(Exception e)
        {
            Notification.Show($"错误 {e.Message}");
        }

        protected abstract Task<IEnumerable<T>> LoadMoreOverride();

        protected abstract Task<Tuple<int, List<T>>> RefreshOverride();
    }
}