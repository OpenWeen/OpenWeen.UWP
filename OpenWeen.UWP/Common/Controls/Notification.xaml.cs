using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OpenWeen.UWP.Common.Controls
{
    public sealed partial class Notification : UserControl
    {
        private static Notification Instance { get; } = new Notification();

        private Notification()
        {
            this.InitializeComponent();
            this.ShowPopup.Completed += async (sender, e) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                this.HidePopup.Begin();
            };
            this.HidePopup.Completed += (sender, e) => popup.IsOpen = false;
        }
        public static void Show(string content)
        {
            Instance.textBlock.Text = content;

            Instance.popup.IsOpen = true;
            Instance.ShowPopup.Begin();
        }

        private void grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (grid.ActualWidth == 0 && grid.ActualHeight == 0)
            {
                return;
            }

            double ActualHorizontalOffset = this.popup.HorizontalOffset;
            double ActualVerticalOffset = this.popup.VerticalOffset;

            double NewHorizontalOffset = (Window.Current.Bounds.Width - grid.ActualWidth) / 2d;
            double NewVerticalOffset = (Window.Current.Bounds.Height - grid.ActualHeight) * 0.75;

            if (ActualHorizontalOffset != NewHorizontalOffset || ActualVerticalOffset != NewVerticalOffset)
            {
                this.popup.HorizontalOffset = NewHorizontalOffset;
                this.popup.VerticalOffset = NewVerticalOffset;
            }
        }
    }
}
