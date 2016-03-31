using System;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Controls.Events;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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
    }
}