using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using OpenWeen.UWP.Common.Helpers;
using OpenWeen.UWP.Model;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace OpenWeen.UWP.Common.Controls
{
    public sealed partial class ImageViewDialog : ContentDialog
    {
        public List<ImageModel> Items { get; internal set; }
        private bool _isPointerPressed;

        private ImageViewDialog()
        {
            this.InitializeComponent();
            MinHeight = (Window.Current.Content as Frame).ActualHeight;
            MinWidth = (Window.Current.Content as Frame).ActualWidth;
            (Window.Current.Content as Frame).SizeChanged += ImageViewDialog_SizeChanged;
        }

        public void HideEx()
        {
            (Window.Current.Content as Frame).SizeChanged -= ImageViewDialog_SizeChanged;
            Hide();
        }

        private void ImageViewDialog_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MinHeight = (Window.Current.Content as Frame).ActualHeight;
            MinWidth = (Window.Current.Content as Frame).ActualWidth;
        }

        public ImageViewDialog(List<ImageModel> items) : this()
        {
            Items = items;
        }

        private void ContentDialog_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            HideEx();
        }

        public void ZoomOut()
        {
            if (flipView?.SelectedItem == null && !(flipView.SelectedItem as ImageModel).IsLoading)
                return;
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem));
            if (scrollViewer.ZoomFactor - 0.1f > scrollViewer.MinZoomFactor)
                scrollViewer.ZoomToFactor(scrollViewer.ZoomFactor - 0.1f);
        }

        public async void Save()
        {
            var name = Path.GetFileName(Items[flipView.SelectedIndex].SourceUri.ToString());
            var file = await KnownFolders.SavedPictures.CreateFileAsync(name);
            using (var client = new HttpClient())
            using (var fstream = await file.OpenStreamForWriteAsync())
            using (var stream = await client.GetStreamAsync(Items[flipView.SelectedIndex].SourceUri))
                await stream.CopyToAsync(fstream);
        }

        public void ZoomIn()
        {
            if (flipView?.SelectedItem == null && !(flipView.SelectedItem as ImageModel).IsLoading)
                return;
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem));
            if (scrollViewer.ZoomFactor + 0.1f < scrollViewer.MaxZoomFactor)
                scrollViewer.ZoomToFactor(scrollViewer.ZoomFactor + 0.1f);
        }

        private Point _prevPoint;

        private void ScrollViewer_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isPointerPressed)
                return;
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem));
            var point = e.GetCurrentPoint(scrollViewer).Position;
            scrollViewer.ChangeView(scrollViewer.HorizontalOffset + (_prevPoint.X - point.X) * 15d, scrollViewer.VerticalOffset + (_prevPoint.Y - point.Y) * 15d, scrollViewer.ZoomFactor, false);
            _prevPoint = point;
        }

        private void ScrollViewer_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isPointerPressed = true;
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem));
            _prevPoint = e.GetCurrentPoint(scrollViewer).Position;
        }

        private void ScrollViewer_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _isPointerPressed = false;
            _prevPoint = default(Point);
        }

        private void ScrollViewer_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isPointerPressed = false;
            _prevPoint = default(Point);
        }
    }
}