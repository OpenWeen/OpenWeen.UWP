using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Controls;
using OpenWeen.UWP.Shared.Common;
using OpenWeen.UWP.View;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace OpenWeen.UWP
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            RequestedTheme = Settings.IsNightMode ? ApplicationTheme.Dark : ApplicationTheme.Light;
            this.Suspending += OnSuspending;
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
            //Microsoft.HockeyApp.HockeyClient.Current.Configure(HockeyAppKey.ApiKey);
            if (StaticResource.IsPhone)
            {
                var statusBar = Type.GetType("Windows.UI.ViewManagement.StatusBar, Windows.Phone.PhoneContract, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime").GetMethod("GetForCurrentView").Invoke(null, null);
                statusBar.GetType().GetProperty("BackgroundColor").SetValue(statusBar, ((SolidColorBrush)Resources["AppTheme"]).Color);
                statusBar.GetType().GetProperty("BackgroundOpacity").SetValue(statusBar, 1d);
                statusBar.GetType().GetProperty("ForegroundColor").SetValue(statusBar, Colors.White);
            }
            else
            {
                ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = ((SolidColorBrush)Resources["AppTheme"]).Color;
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = ((SolidColorBrush)Resources["AppTheme"]).Color;
            }
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
                    ExtendedSplash extendedSplash = new ExtendedSplash(e.SplashScreen, loadState);
                    Window.Current.Content = extendedSplash;
                }
            }
            Window.Current.Activate();
            UnhandledException += App_UnhandledException;
            RegisterExceptionHandlingSynchronizationContext();
        }
        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            RegisterExceptionHandlingSynchronizationContext();
        }
        private void RegisterExceptionHandlingSynchronizationContext()
        {
            ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException;
        }

        private void SynchronizationContext_UnhandledException(object sender, Common.UnhandledExceptionEventArgs e)
        {
            if (e.Exception is WebException || e.Exception is HttpRequestException)
            {
                Notification.Show($"网络错误 {e.Exception.Message}");
                e.Handled = true;
            }
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            if (e.Exception is WebException || e.Exception is HttpRequestException)
            {
                Notification.Show($"网络错误 {e.Exception.Message}");
                e.Handled = true;
            }
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
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}