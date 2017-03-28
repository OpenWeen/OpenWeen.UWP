using System;
using System.ComponentModel;
using System.IO;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenWeen.UWP.Model
{
    public class ImageModel : INotifyPropertyChanged
    {
        public string SourceUri { get; set; }
        public BitmapImage Image { get; set; }
        public int DownloadProgress { get; private set; }
        public Visibility ProgressVisibility { get; private set; } = Visibility.Visible;

        public event PropertyChangedEventHandler PropertyChanged;

        public ImageModel(string source, bool autoPlay = true)
        {
            SourceUri = source;
            Image = new BitmapImage(new Uri(source))
            {
                AutoPlay = autoPlay
            };
            Image.DownloadProgress += Image_DownloadProgress;
        }

        private void Image_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            DownloadProgress = e.Progress;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadProgress)));
        }

        public void Loaded(object sender, object e)
        {
            ProgressVisibility = Visibility.Collapsed;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressVisibility)));
        }
    }
}