using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using OpenWeen.UWP.Common.Helpers;
using OpenWeen.UWP.Model;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WinRTXamlToolkit.Controls.Extensions;

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
            InitSize();
            Window.Current.SizeChanged += Current_SizeChanged;
            DisplayInformation.GetForCurrentView().OrientationChanged += ExtendedSplash_OrientationChanged;
        }

        private void ExtendedSplash_OrientationChanged(DisplayInformation sender, object args)
        {
            InitSize();
        }


        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            InitSize();
        }

        public void HideEx()
        {
            DisplayInformation.GetForCurrentView().OrientationChanged -= ExtendedSplash_OrientationChanged;
            Window.Current.SizeChanged -= Current_SizeChanged;
            Hide();
        }
        
        private async void InitSize()
        {
            MinHeight = (Window.Current.Content as Frame).ActualHeight;
            MinWidth = (Window.Current.Content as Frame).ActualWidth;
            await InitImageSize();
        }

        private async System.Threading.Tasks.Task InitImageSize()
        {
            await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var img = MoreVisualTreeHelper.GetObject<Image>(flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem));
                if (img != null)
                {
                    img.Width = (Window.Current.Content as Frame).ActualWidth;
                    img.Height = (Window.Current.Content as Frame).ActualHeight;
                }
            });
        }

        private ImageViewDialog(List<ImageModel> items) : this()
        {
            Items = items;
        }
        public ImageViewDialog(List<ImageModel> items, int index) : this(items)
        {
            _index = index;
        }

        private void ContentDialog_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            HideEx();
        }

        public void ZoomOut()
        {
            if (flipView?.SelectedItem == null && (flipView.SelectedItem as ImageModel).IsLoading)
                return;
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem));
            if (scrollViewer.ZoomFactor - 0.1f > scrollViewer.MinZoomFactor)
                scrollViewer.ZoomToFactorWithAnimationAsync(scrollViewer.ZoomFactor - 0.1f, 0.5);
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
            if (flipView?.SelectedItem == null && (flipView.SelectedItem as ImageModel).IsLoading)
                return;
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem));
            if (scrollViewer.ZoomFactor + 0.1f < scrollViewer.MaxZoomFactor)
                scrollViewer.ZoomToFactorWithAnimationAsync(scrollViewer.ZoomFactor + 0.1f, 0.5);
        }

        private Point _prevPoint;
        private int _index;

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

        private void flipView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            flipView.SelectedIndex = _index;
        }

        private async void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await InitImageSize();
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideEx();
        }
    }
}