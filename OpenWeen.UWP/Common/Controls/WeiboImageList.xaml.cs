using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public sealed partial class WeiboImageList : UserControl
    {
        public event EventHandler<WeiboPictureClickEventArgs> PictureClick;



        public IList<PictureModel> ItemsSource
        {
            get { return (IList<PictureModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList<PictureModel>), typeof(WeiboImageList), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WeiboImageList).InitImage(e.NewValue as IList<PictureModel>);
        }

        private void InitImage(IList<PictureModel> list)
        {
            foreach (var item in root.Children)
                item.Visibility = Visibility.Collapsed;
            Grid grid;
            if (list == null)
                return;
            switch (list.Count)
            {
                case 0:
                    return;
                case 1:
                    grid = OneGrid;
                    break;
                case 2:
                    grid = TwoGrid;
                    break;
                case 3:
                    grid = ThreeGrid;
                    break;
                case 4:
                    grid = FourGird;
                    break;
                case 5:
                case 6:
                    grid = SixGrid;
                    break;
                case 7:
                case 8:
                case 9:
                default:
                    grid = NineGrid;
                    break;
            }
            foreach (Image item in grid.Children)
                item.DataContext = null;
            grid.Visibility = Visibility.Visible;
            for (int i = 0; i < Math.Min(9, list.Count); i++)
            {
                var item = list[i];
                var img = VisualTreeHelper.GetChild(grid, i) as FrameworkElement;
                img.DataContext = item;
            }
            InitSize(grid, ActualWidth < 720 ? ActualWidth : 720);
        }

        public WeiboImageList()
        {
            this.InitializeComponent();
            Loaded += WeiboImageList_Loaded;
            SizeChanged += WeiboImageList_SizeChanged;
        }

        private void WeiboImageList_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = root.Children.Where(item => item.Visibility == Visibility.Visible).FirstOrDefault() as Grid;
            if (grid != null)
                InitSize(grid, ActualWidth < 720 ? ActualWidth : 720);
        }

        private void WeiboImageList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width != e.NewSize.Width && ItemsSource?.Count > 0)
            {
                var width = e.NewSize.Width;
                var grid = root.Children.Where(item => item.Visibility == Visibility.Visible).FirstOrDefault() as Grid;
                if (grid != null)
                    InitSize(grid, width < 720 ? width : 720);
            }
        }

        private void InitSize(FrameworkElement grid, double width)
        {
            switch (ItemsSource.Count)
            {
                case 1:
                    grid.MinHeight = width > 480 ? width / 3 : width;
                    break;
                case 2:
                    grid.MinHeight = width / 3;
                    break;
                case 4:
                case 5:
                case 6:
                    grid.MinHeight = width / 3 * 2;
                    break;
                case 3:
                case 7:
                case 8:
                case 9:
                default:
                    grid.MinHeight = width;
                    break;
            }
            grid.MaxHeight = grid.MinHeight;
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            PictureClick?.Invoke(this, new WeiboPictureClickEventArgs((sender as Image).DataContext as PictureModel) { DataContext = DataContext as MessageModel });
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