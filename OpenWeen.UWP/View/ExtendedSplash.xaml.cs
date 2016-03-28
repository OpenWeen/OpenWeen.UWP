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
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using System.Threading.Tasks;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Helpers;
using ImageLib;
using ImageLib.Cache.Memory;
using ImageLib.Cache.Storage.CacheImpl;
using Windows.Storage;
using ImageLib.Cache.Storage;
using ImageLib.Gif;
using Windows.Storage.Streams;
using OpenWeen.UWP.Shared.Common.Helpers;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.BackgroundTask;
using Windows.ApplicationModel.Background;


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OpenWeen.UWP.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    partial class ExtendedSplash : Page
    {
        internal Rect splashImageRect;
        private SplashScreen splash;
        internal bool dismissed = false;
        internal Frame rootFrame;

        public ExtendedSplash(SplashScreen splashscreen, bool loadState)
        {
            InitializeComponent();
            Window.Current.SizeChanged += ExtendedSplash_OnResize;
            splash = splashscreen;
            if (splash != null)
            {
                splash.Dismissed += DismissedEventHandler;
                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
                PositionTextBlock();
            }
            rootFrame = new Frame();
            RestoreStateAsync(loadState);
        }

        private async Task InitEmotion()
        {
            StaticResource.Emotions = (await Core.Api.Statuses.Emotions.GetEmotions()).ToList();
            StaticResource.EmotionPattern = string.Join("|", StaticResource.Emotions.Select(item => item.Value)).Replace("[", @"\[").Replace("]", @"\]");
        }
        private bool CheckForLogin()
        {
            try
            {
                Core.Api.Entity.AccessToken = SettingHelper.GetListSetting<string>(SettingNames.AccessToken, isThrowException: true).ToList()[0];
                return true;
            }
            catch (SettingException)
            {
                return false;
            }
        }
        void RestoreStateAsync(bool loadState)
        {
            if (loadState)
            {
                // TODO: write code to load state
            }
        }
        void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;

        }

        void PositionRing()
        {
            splashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5));
            splashProgressRing.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1));
        }

        void PositionTextBlock()
        {
            textBlock.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5) - 145d * 0.5);
            textBlock.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1) + 28);
        }

        void ExtendedSplash_OnResize(object sender, WindowSizeChangedEventArgs e)
        {
            if (splash != null)
            {
                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
                PositionTextBlock();
            }
        }

        async void DismissedEventHandler(SplashScreen sender, object e)
        {
            dismissed = true;

            ImageLoader.Initialize(new ImageConfig.Builder()
            {
                CacheMode = ImageLib.Cache.CacheMode.MemoryAndStorageCache,
                MemoryCacheImpl = new LRUCache<string, IRandomAccessStream>(),
                StorageCacheImpl = new LimitedStorageCache(ApplicationData.Current.LocalCacheFolder,
              "cache", new SHA1CacheGenerator(), 1024 * 1024 * 1024)
            }.AddDecoder<GifDecoder>().Build(), false);
            await BackgroundHelper.Register<UpdateUnreadCountTask>(new TimeTrigger(15, false));
            if (CheckForLogin())
            {
                await InitEmotion();
                await InitUid();
            }
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 DismissExtendedSplash();
             });
        }

        private async Task InitUid()
        {
            StaticResource.Uid = long.Parse(await Core.Api.User.Account.GetUid());
        }

        void DismissExtendedSplash()
        {
            Window.Current.SizeChanged -= ExtendedSplash_OnResize;
            if (CheckForLogin())
            {
                rootFrame.Navigate(typeof(MainPage));
            }
            else
            {
                rootFrame.Navigate(typeof(LoginPage));
            }
            while (rootFrame.BackStack.Count > 0)
            {
                rootFrame.BackStack.RemoveAt(0);
            }
            SystemNavigationManager.GetForCurrentView().BackRequested += ExtendedSplash_BackRequested;
            rootFrame.Navigated += RootFrame_Navigated;
            Window.Current.Content = rootFrame;
        }

        private void ExtendedSplash_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (rootFrame.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }
    }

}
