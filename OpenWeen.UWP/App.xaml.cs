using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.HockeyApp;
using Newtonsoft.Json;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Controls;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.Shared.Common.Helpers;
using OpenWeen.UWP.View;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Composition;
using System.Numerics;

namespace OpenWeen.UWP
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        private static Compositor _compositor;
        private static SpriteVisual _hostSprite;

        public static Color AppTheme => ((SolidColorBrush)App.Current.Resources["AppTheme"]).Color;
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            RequestedTheme = GetTheme();
            this.Suspending += OnSuspending;
        }

        private ApplicationTheme GetTheme()
        {
            if (Settings.IsAutoNightMode && (DateTime.Now.TimeOfDay > Settings.AutoNightModeOnTime || DateTime.Now.TimeOfDay < Settings.AutoNightModeOffTime))
                return ApplicationTheme.Dark;
            return Settings.IsNightMode ? ApplicationTheme.Dark : ApplicationTheme.Light;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            HockeyClient.Current.Configure(HockeyAppKey.ApiKey);
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null && e.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                if (StaticResource.IsPhone)
                {
                    StatusBar.GetForCurrentView().BackgroundColor = ((SolidColorBrush)Resources["AppTheme"]).Color;
                    StatusBar.GetForCurrentView().BackgroundOpacity = 1d;
                    StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
                }
                else
                {
                    ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = ((SolidColorBrush)Resources["TitleBarColor"]).Color;
                    ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = ((SolidColorBrush)Resources["TitleBarColor"]).Color;
                    ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(350, 200));
                }
                Window.Current.Content = new ExtendedSplash(e.SplashScreen);
            }

            //Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
            //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
            //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Colors.Transparent;
            //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.Black;
            Window.Current.Activate();
#if !DEBUG
            UnhandledException += App_UnhandledException;
            RegisterExceptionHandlingSynchronizationContext(); 
#endif
        }
        private void RegisterExceptionHandlingSynchronizationContext()
        {
            ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException;
        }

        internal static void HandleBackButton(Frame rootFrame)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += (sender, e) =>
            {
                if (rootFrame.CanGoBack)
                {
                    e.Handled = true;
                    rootFrame.GoBack();
                }
                else if (StaticResource.IsPhone)
                {
                    Current.Exit();
                }
            };
            rootFrame.Navigated += (sender, e) =>
            {
                if (rootFrame.CanGoBack)
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                else
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            };
        }

        private void SynchronizationContext_UnhandledException(object sender, Common.UnhandledExceptionEventArgs e)
        {
            e.Handled = HandleException(e.Exception);
        }

        private bool HandleException(Exception exception)
        {
            if (exception is WebException || exception is HttpRequestException)
            {
                Notification.Show($"网络错误 {exception.Message}");
                return true;
            } 
            else if (exception is JsonException)
            {
                Notification.Show($"Json错误 {exception.Message}");
                return true;
            }
            else if (exception is TaskCanceledException)
            {
                Notification.Show("超时");
                return true;
            }
            return false;
        }

        internal static void InitBlurEffect(Grid grid)
        {
            _compositor = ElementCompositionPreview
               .GetElementVisual(grid).Compositor;
            grid.Loaded += (sender, e) =>
            {
                _hostSprite = _compositor.CreateSpriteVisual();
                _hostSprite.Size = new Vector2(
                    (float)grid.ActualWidth,
                    (float)grid.ActualHeight);
                ElementCompositionPreview.SetElementChildVisual(
                    grid, _hostSprite);
                _hostSprite.Brush = _compositor.CreateHostBackdropBrush();
            };
            grid.SizeChanged += (sender, e) =>
            {
                if (_hostSprite != null)
                    _hostSprite.Size = e.NewSize.ToVector2();
            };
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = HandleException(e.Exception);
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>1
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            var frame = Window.Current.Content as Frame ?? new Frame();
            Window.Current.Content = frame;
            if (Core.Api.Entity.AccessToken == null)
                Core.Api.Entity.AccessToken = SettingHelper.GetListSetting<string>(SettingNames.AccessToken).ToList()[Settings.SelectedUserIndex];
            if (StaticResource.Emotions == null)
                await StaticResource.InitEmotion();
            frame.Navigate(typeof(PostWeiboPage), new Common.Entities.PostWeibo());
            //ShareOperation shareOperation = args.ShareOperation;
            //if (shareOperation.Data.Contains(StandardDataFormats.Text))
            //{
            //    string text = await shareOperation.Data.GetTextAsync();
            //    frame.Navigate(typeof(PostWeiboPage), new Common.Entities.SharedPostWeibo() { Data = text, Operation = shareOperation });
            //    shareOperation.ReportDataRetrieved();
            //}
            //else if (shareOperation.Data.Contains(StandardDataFormats.Bitmap))
            //{
            //    var bitmap = await shareOperation.Data.GetBitmapAsync();
            //    using (var stream = await bitmap.OpenReadAsync())
            //    {
            //        byte[] bytes = new byte[stream.Size];
            //        var buffer = await stream.ReadAsync(bytes.AsBuffer(), (uint)stream.Size, InputStreamOptions.None);
            //        bytes = buffer.ToArray();
            //        frame.Navigate(typeof(PostWeiboPage), new Common.Entities.SharedPostWeibo() { ImageData = bytes, Operation = shareOperation });
            //    }
            //    shareOperation.ReportDataRetrieved();
            //}
            //else if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            //{
            //    var files = await shareOperation.Data.GetStorageItemsAsync();
            //    frame.Navigate(typeof(PostWeiboPage), new Common.Entities.SharedPostWeibo() { Operation = shareOperation, ImageFiles = files });
            //}
            Window.Current.Activate();
        }
    }
}