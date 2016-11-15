using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Controls.Events;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OpenWeen.UWP.Common.Controls
{
    public sealed partial class WeiboImageList : ItemsControl
    {
        public event EventHandler<WeiboPictureClickEventArgs> PictureClick;

        public WeiboImageList()
        {
            this.InitializeComponent();
        }
        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            PictureClick?.Invoke(this, new WeiboPictureClickEventArgs((sender as Image).DataContext as PictureModel));
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var image = sender as Image;
            var url = (image.Source as BitmapImage).UriSource.ToString();
            if (url.Contains("/or180/"))
            {
                url = url.Replace("/or180/", "/thumbnail/");
                image.Source = new BitmapImage(new Uri(url));
            }
            else if (url.Contains("/or360/"))
            {
                url = url.Replace("/or360/", "/bmiddle/");
                image.Source = new BitmapImage(new Uri(url));
            }
        }
    }
}