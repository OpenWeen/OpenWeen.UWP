using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.Extensions;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace OpenWeen.UWP.Common.Controls
{
    public sealed partial class WeiboVideoPlayer : ContentDialog
    {
        private string _oriurl;
        public WeiboVideoPlayer(string oriurl, string url)
        {
            this.InitializeComponent();
            this._oriurl = oriurl;
            InitSize();
            mediaElement.Source = new Uri(url);
            Window.Current.SizeChanged += Current_SizeChanged;
            DisplayInformation.GetForCurrentView().OrientationChanged += ExtendedSplash_OrientationChanged;
            Closed += WeiboVideoPlayer_Closed;
        }

        private void WeiboVideoPlayer_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            DisplayInformation.GetForCurrentView().OrientationChanged -= ExtendedSplash_OrientationChanged;
            Window.Current.SizeChanged -= Current_SizeChanged;
            Closed -= WeiboVideoPlayer_Closed;
        }

        private void ExtendedSplash_OrientationChanged(DisplayInformation sender, object args)
        {
            InitSize();
        }

        private void InitSize()
        {
            MinHeight = (Window.Current.Content as Frame).ActualHeight;
            MinWidth = (Window.Current.Content as Frame).ActualWidth;
            mediaElement.Height = (Window.Current.Content as Frame).ActualHeight;
            mediaElement.Width = (Window.Current.Content as Frame).ActualWidth;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            InitSize();
        }
        
        private void ContentDialog_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

        }

        private async void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(_oriurl));
        }
    }
}
