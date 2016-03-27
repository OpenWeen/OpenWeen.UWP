using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace OpenWeen.UWP.Model
{
    public class HeaderModel : INotifyPropertyChanged
    {
        public Symbol Icon { get; internal set; }
        public string Text { get; internal set; }
        private int _unreadCount = 0;

        public int UnreadCount
        {
            get { return _unreadCount; }
            internal set
            {
                _unreadCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnreadCount)));
            }
        }

        public double Opacity => IsActive ? 1d : 0.37;
        private bool _isActive;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Opacity)));
            }
        }

    }
}
