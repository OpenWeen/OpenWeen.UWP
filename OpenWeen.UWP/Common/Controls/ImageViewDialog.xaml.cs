using OpenWeen.UWP.Common.Helpers;
using OpenWeen.UWP.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
        }
        
        public ImageViewDialog(List<ImageModel> items) : this()
        {
            Items = items;
        }
        private void ContentDialog_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Hide();
        }
        public void ZoomOut()
        {
            if (flipView?.SelectedItem == null && !(flipView.SelectedItem as ImageModel).IsLoading)
                return;
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem));
            if (scrollViewer.ZoomFactor - 0.1f > scrollViewer.MinZoomFactor)
            {
                scrollViewer.ZoomToFactor(scrollViewer.ZoomFactor - 0.1f);
            }
        }
        public void ZoomIn()
        {
            if (flipView?.SelectedItem == null && !(flipView.SelectedItem　as ImageModel).IsLoading)
                return;
            var scrollViewer = MoreVisualTreeHelper.GetObject<ScrollViewer>(flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem));
            if (scrollViewer.ZoomFactor + 0.1f < scrollViewer.MaxZoomFactor)
            {
                scrollViewer.ZoomToFactor(scrollViewer.ZoomFactor + 0.1f);
            }
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
