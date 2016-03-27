using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Controls.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace OpenWeen.UWP.ViewModel
{
    public abstract class WeiboListViewModelBase<T> : INotifyPropertyChanged where T : BaseModel
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
            catch (NullReferenceException e)
            {
                await new MessageDialog(e.Message + e.StackTrace).ShowAsync();
                throw;
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


        private async void OnWebException()
        {
            await new MessageDialog("网络错误").ShowAsync();
        }

        protected abstract Task<IEnumerable<T>> LoadMoreOverride();

        protected abstract Task<IEnumerable<T>> RefreshOverride();
        
    }
}
