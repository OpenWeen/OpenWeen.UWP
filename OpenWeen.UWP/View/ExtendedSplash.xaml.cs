using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenWeen.Core.Model;
//using ImageLib;
//using ImageLib.Cache.Memory;
//using ImageLib.Cache.Storage;
//using ImageLib.Cache.Storage.CacheImpl;
//using ImageLib.Gif;
using OpenWeen.UWP.BackgroundTask;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Helpers;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

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
        internal Frame rootFrame = new Frame();

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
            InitTransitions();
            RestoreStateAsync(loadState);
        }
        private void InitTransitions()
        {
            TransitionCollection collection = new TransitionCollection();
            NavigationThemeTransition theme = new NavigationThemeTransition();

            var info = new DrillInNavigationTransitionInfo();

            theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);
            rootFrame.ContentTransitions = collection;
        }

        private async Task InitEmotion()
        {
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("assets\\emotion.json");
            var text = File.ReadAllText(file.Path);
            StaticResource.Emotions = JsonConvert.DeserializeObject<IEnumerable<EmotionModel>>(text).ToList();
            StaticResource.EmotionPattern = string.Join("|", StaticResource.Emotions.Select(item => item.Value)).Replace("[", @"\[").Replace("]", @"\]");
        }

        private bool CheckForLogin()
        {
            try
            {
                Core.Api.Entity.AccessToken = SettingHelper.GetListSetting<string>(SettingNames.AccessToken, isThrowException: true).FirstOrDefault();
                if (string.IsNullOrEmpty(Core.Api.Entity.AccessToken))
                {
                    throw new Core.Exception.InvalidAccessTokenException();
                }
                return true;
            }
            catch (Exception e) when (e is Core.Exception.InvalidAccessTokenException || e is SettingException)
            {
                return false;
            }

        }

        private void RestoreStateAsync(bool loadState)
        {
            if (loadState)
            {
                // TODO: write code to load state
            }
        }

        private void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;
        }

        private void PositionRing()
        {
            splashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5));
            splashProgressRing.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1));
        }

        private void PositionTextBlock()
        {
            textBlock.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5) - 145d * 0.5);
            textBlock.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1) + 28);
        }

        private void ExtendedSplash_OnResize(object sender, WindowSizeChangedEventArgs e)
        {
            if (splash != null)
            {
                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
                PositionTextBlock();
            }
        }

        private async void DismissedEventHandler(SplashScreen sender, object e)
        {
            dismissed = true;
            await BackgroundHelper.Register<UpdateUnreadCountTask>(new TimeTrigger(15, false));
            await InitEmotion();
            if (CheckForLogin())
            {
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

        private void DismissExtendedSplash()
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
            SystemNavigationManager.GetForCurrentView().BackRequested += ExtendedSplash_BackRequested;
            rootFrame.Navigated += RootFrame_Navigated;
            Window.Current.Content = rootFrame;
        }

        private void ExtendedSplash_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
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