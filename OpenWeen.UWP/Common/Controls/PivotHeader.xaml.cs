using OpenWeen.UWP.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OpenWeen.UWP.Common.Controls
{
    public sealed partial class PivotHeader : UserControl
    {
        public event EventHandler BackToTop;
        public event EventHandler Refresh;

        public List<HeaderModel> ItemsSource { get; set; }
        public PivotHeader()
        {
            this.InitializeComponent();
        }



        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(PivotHeader), new PropertyMetadata(-1,OnSelectedIndexChanged));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PivotHeader).ChangeSelectedIndex((int)e.NewValue, (int)e.OldValue);
        }
        public void ChangeSelectedIndex(int newValue,int oldValue)
        {
            try
            {
                ItemsSource[newValue].IsActive = true;
                if (ItemsSource[newValue].UnreadCount > 0)
                {
                    ItemsSource[newValue].UnreadCount = 0;
                    Refresh?.Invoke(this, new EventArgs());
                }
                ItemsSource[oldValue].IsActive = false;
            }
            catch { }
        }

        private void ItemsControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var index = ItemsSource.FindIndex(item => item == (e.OriginalSource as FrameworkElement).DataContext as HeaderModel);
            if (SelectedIndex == index)
            {
                BackToTop?.Invoke(this, new EventArgs());
            }
            else
            {
                SelectedIndex = index;
            }
        }
    }
}
