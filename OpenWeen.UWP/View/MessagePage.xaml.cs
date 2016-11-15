using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using OpenWeen.Core.Model;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.ViewModel;
using OpenWeen.UWP.ViewModel.MessagePage;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace OpenWeen.UWP.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MessagePage : Page
    {
        public MessagePageViewModel ViewModel { get; private set; }
        public MessagePage()
        {
            this.InitializeComponent();
            if (StaticResource.EmojiGroup != null)
            {
                cvs.Source = StaticResource.EmojiGroup;
                (semanticZoom.ZoomedOutView as ListViewBase).ItemsSource = cvs.View.CollectionGroups;
            }
        }
        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as MessagePageViewModel;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as EmotionModel;
            var index = textBox.SelectionStart;
            textBox.Text = textBox.Text.Insert(index, item.Value);//TODO:pure emotion can not send message
            textBox.SelectionStart = index + item.Value.Length;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ViewModel?.CleanUp();
        }
    }
}
