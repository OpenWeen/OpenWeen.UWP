using ImageLib.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeen.UWP.Model
{
    public class ImageModel : INotifyPropertyChanged
    {
        public string SourceUri { get; }
        public bool IsLoading { get; private set; } = true;
        public event PropertyChangedEventHandler PropertyChanged;
        public ImageModel(string source)
        {
            SourceUri = source;
        }

        public void Loaded()
        {
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        public void LoadFailed(object sender, object e)
        {
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            var imageView = sender as ImageView;
            //TODO: Draw an image
            //imageView.UriSource = new Uri()
        }
    }
}
