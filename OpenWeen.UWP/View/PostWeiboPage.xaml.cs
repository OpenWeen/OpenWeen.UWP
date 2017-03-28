using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Status;
using OpenWeen.Core.Model.Types;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Controls;
using OpenWeen.UWP.Common.Entities;
using PropertyChanged;
using Windows.Foundation.Metadata;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using OpenWeen.Core.Exception;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OpenWeen.UWP.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PostWeiboPage : Page, INotifyPropertyChanged
    {

        public PostWeiboPage()
        {
            this.InitializeComponent();
            Transitions = new TransitionCollection { new NavigationThemeTransition() { DefaultNavigationTransitionInfo = new SlideNavigationTransitionInfo() } };
            if (StaticResource.Emotions != null)
            {
                cvs.Source = StaticResource.EmojiGroup;
                (semanticZoom.ZoomedOutView as ListViewBase).ItemsSource = cvs.View.CollectionGroups;
            }
        }

        public ObservableCollection<ImageData> Images { get; } = new ObservableCollection<ImageData>();
        private IPostWeibo _data;
        public bool AllowMorePicture { get; set; } = true;
        public WeiboVisibility WeiboVisibility { get; set; } = WeiboVisibility.All;
        public SymbolIcon WeiboVisibilityIcon
        {
            get
            {
                switch (WeiboVisibility)
                {
                    case WeiboVisibility.All:
                        return new SymbolIcon(Symbol.World);
                    case WeiboVisibility.OnlyMe:
                        return new SymbolIcon(Symbol.Contact);
                    case WeiboVisibility.OnlyFriends:
                        return new SymbolIcon(Symbol.People);
                    case WeiboVisibility.SpecifiedGroup:
                        return new SymbolIcon(Symbol.People);
                    default:
                        return new SymbolIcon(Symbol.Permissions);
                }
            }
        }
        private bool _isSending;

        public int TextCount
        {
            get
            {
                richEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.NoHidden, out var value);
                return 140 - Encoding.GetEncoding("gb2312").GetByteCount(value) / 2;
            }
        }

        public string Text
        {
            get
            {
                richEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.NoHidden, out string value);
                return value;
            }
            set
            {
                richEditBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, value);
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter is IPostWeibo)) return;
            _data = e.Parameter as IPostWeibo;
            AllowMorePicture = _data.Type == PostWeiboType.NewPost;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllowMorePicture)));
            if (_data is IPostWeiboData)
            {
                var data = _data as IPostWeiboData;
                if (!string.IsNullOrEmpty(data.Data))
                {
                    Text = data.Data;
                    if (_data.Type == PostWeiboType.RePost)
                        richEditBox.Document.Selection.StartPosition = 0;
                    else
                        richEditBox.Document.Selection.StartPosition = Text.Length;
                }
            }
            if (_data is SharedPostWeibo)
            {
                if ((_data as SharedPostWeibo).ImageData != null)
                    await AddImageData((_data as SharedPostWeibo).ImageData);
                else if ((_data as SharedPostWeibo).ImageFiles != null)
                    foreach (var item in (_data as SharedPostWeibo).ImageFiles)
                        if (item is StorageFile)
                            await AddImageDataFromFile(item as StorageFile);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void RichEditBox_Paste(object sender, TextControlPasteEventArgs e)
        {
            if (Images.Count >= 9)
                return;
            var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Bitmap))
            {
                e.Handled = true;
                var bitmap = await dataPackageView.GetBitmapAsync();
                var file = await GetFileFromBitmap(bitmap);
                await AddImageDataFromFile(file, true);
            }
            else if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
            {
                e.Handled = true;
                var files = (await dataPackageView.GetStorageItemsAsync()).Where(item => item is StorageFile && (item as StorageFile).ContentType.Contains("image")).ToList();
                if (AllowMorePicture)
                    for (int i = 0; i < 9 && i < files.Count; i++)
                        await AddImageDataFromFile(files[i] as StorageFile);
                else
                    await AddImageDataFromFile(files[0] as StorageFile);
            }
        }

        private static async Task<StorageFile> GetFileFromBitmap(RandomAccessStreamReference bitmap)
        {
            var file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"{new Random().Next()}.jpg", CreationCollisionOption.GenerateUniqueName);
            using (var fstream = await file.OpenStreamForWriteAsync())
            using (var stream = await bitmap.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var pixels = await decoder.GetPixelDataAsync();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, fstream.AsRandomAccessStream());
                encoder.SetPixelData(decoder.BitmapPixelFormat, BitmapAlphaMode.Ignore,
                    decoder.OrientedPixelWidth, decoder.OrientedPixelHeight,
                    decoder.DpiX, decoder.DpiY,
                    pixels.DetachPixelData());
                await encoder.FlushAsync();
            }
            return file;
        }

        private async Task AddImageData(byte[] data)
        {
            if (!(data.Length / 1024f / 1024f > 5f))//The image is larger than 5MB
            {
                using (var stream = new MemoryStream(data))
                {
                    var b = new BitmapImage();
                    await b.SetSourceAsync(stream.AsRandomAccessStream());
                    if (!AllowMorePicture && Images.Count > 0)
                        Images.RemoveAt(0);
                    Images.Add(new ImageData() { Data = data, Image = b });
                }
            }
            else
                Notification.Show("图片不能大于5MB");
        }
        [ImplementPropertyChanged]
        public class ImageData
        {
            public BitmapImage Image { get; set; }
            [DoNotNotify]
            public byte[] Data { get; set; }
        }

        private void RichEditBox_TextChanged(object sender, RoutedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextCount)));
        }

        public async void PostWeibo()
        {
            if (_isSending || Text?.Length < 0)
                return;
            if (TextCount < 0)
            {
                var dialog = new MessageDialog("超出140字限制", "错误");
                dialog.Commands.Add(new UICommand("删除超出部分并发送", (command) =>
                {
                    Text = Text.Remove(139);
                    PostWeibo();
                }));
                dialog.Commands.Add(new UICommand("取消"));
                await dialog.ShowAsync();
                return;
            }
            _isSending = true;
            var sardialog = new SitbackAndRelaxDialog();
            sardialog.ShowAsync();
            var (isSuccess, message) = (false, "");
            switch (_data.Type)
            {
                case PostWeiboType.NewPost:
                    (isSuccess, message) = await NewPost();
                    break;

                case PostWeiboType.RePost:
                    (isSuccess, message) = await RePost();
                    break;

                case PostWeiboType.Comment:
                    (isSuccess, message) = await Comment();
                    break;

                default:
                    break;
            }
            sardialog.Hide();
            if (isSuccess)
            {
                if (_data is SharedPostWeibo)
                    (_data as SharedPostWeibo).Operation.ReportCompleted();
                else
                    Frame.GoBack();
            }
            else
                Notification.Show(message);
            _isSending = false;
        }

        private async Task<(bool, string)> Comment()
        {
            if (_data is ReplyCommentData)
            {
                var data = _data as ReplyCommentData;
                try
                {
                    if (Images.Count > 0)
                        await Core.Api.Comments.ReplyWithPic(data.ID, data.CID, Text, new MediaModel((await Core.Api.Statuses.PostWeibo.UploadPicture(Images.FirstOrDefault().Data)).PicID));
                    else
                        await Core.Api.Comments.Reply(data.ID, data.CID, Text);
                    return (true, "");
                }
                catch (Exception e) when (e is WebException || e is System.Net.Http.HttpRequestException) 
                {
                    return (false, "发送失败，请检查网络");
                }
                catch (WeiboException e)
                {
                    return (false, e.Message);
                }
                catch (TaskCanceledException)
                {
                    return (false, "发送超时");
                }
            }
            else if (_data is CommentData)
            {
                var data = _data as CommentData;
                try
                {
                    if (Images.Count > 0)
                        await Core.Api.Comments.PostCommentWithPic(data.ID, Text, new MediaModel((await Core.Api.Statuses.PostWeibo.UploadPicture(Images.FirstOrDefault().Data)).PicID));
                    else
                        await Core.Api.Comments.PostComment(data.ID, Text);
                    return (true, "");
                }
                catch (Exception e) when (e is WebException || e is System.Net.Http.HttpRequestException)
                {
                    return (false, "发送失败，请检查网络");
                }
                catch (WeiboException e)
                {
                    return (false, e.Message);
                }
                catch (TaskCanceledException)
                {
                    return (false, "发送超时");
                }
            }
            return (false, "发送失败");
        }

        private async Task<(bool, string)> RePost()
        {
            var data = _data as RepostData;
            try
            {
                if (Images.Count > 0)
                    await Core.Api.Statuses.PostWeibo.RepostWithPic(data.ID, Text, new MediaModel((await Core.Api.Statuses.PostWeibo.UploadPicture(Images.FirstOrDefault().Data)).PicID));
                else
                    await Core.Api.Statuses.PostWeibo.Repost(data.ID, Text);
                return (true, "");
            }
            catch (Exception e) when (e is WebException || e is System.Net.Http.HttpRequestException)
            {
                return (false, "发送失败，请检查网络");
            }
            catch (WeiboException e)
            {
                return (false, e.Message);
            }
            catch (TaskCanceledException)
            {
                return (false, "发送超时");
            }
        }

        private async Task<(bool, string)> NewPost()
        {
            try
            {
                if (Images.Count > 0)
                {
                    var pics = new List<PictureModel>();
                    foreach (var item in Images)
                        pics.Add(await Core.Api.Statuses.PostWeibo.UploadPicture(item.Data));
                    await Core.Api.Statuses.PostWeibo.PostWithMultiPics(Text?.Length > 0 ? Text : "分享图片", string.Join(",", pics.Select(item => item.PicID)), visible: WeiboVisibility);
                    return (true, "");
                }
                else if (Text?.Length > 0)
                {
                    await Core.Api.Statuses.PostWeibo.Post(Text, visible: WeiboVisibility);
                    return (true, "");
                }
            }
            catch (Exception e) when (e is WebException || e is System.Net.Http.HttpRequestException)
            {
                return (false, "发送失败，请检查网络");
            }
            catch (WeiboException e)
            {
                return (false, e.Message);
            }
            catch (TaskCanceledException)
            {
                return (false, "发送超时");
            }
            return (false, "发送失败");
        }
        
        private async void richEditBox_DragOver(object sender, DragEventArgs e)
        {
            var def = e.GetDeferral();
            e.Handled = true;
            if (e.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Bitmap))
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            else if (e.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
            {
                var files = (await e.DataView.GetStorageItemsAsync()).Where(item => item is StorageFile && (item as StorageFile).ContentType.Contains("image")).ToList();
                e.AcceptedOperation = files?.Count > 0 && Images.Count < 9 ?
                    Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy :
                    Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
            }
            def.Complete();
        }

        private async void richEditBox_Drop(object sender, DragEventArgs e)
        {
            if (!AllowMorePicture)
                return;
            var def = e.GetDeferral();
            e.Handled = true;
            if (e.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Bitmap))
                await AddImageDataFromFile(await GetFileFromBitmap(await e.DataView.GetBitmapAsync()));
            else if (e.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
            {
                var files = (await e.DataView.GetStorageItemsAsync()).Where(item => item is StorageFile && (item as StorageFile).ContentType.Contains("image")).ToList();
                if (AllowMorePicture)
                    for (int i = 0; i < 9 && i < files.Count; i++)
                        await AddImageDataFromFile(files[i] as StorageFile);
                else
                    await AddImageDataFromFile(files[0] as StorageFile);
            }
            def.Complete();
        }

        private async Task AddImageDataFromFile(StorageFile file, bool deleteAfterUsed = false)
        {
            byte[] data;
            using (var stream = await file.OpenReadAsync())
            using (DataReader dataReader = new DataReader(stream))
            {
                data = new byte[stream.Size];
                await dataReader.LoadAsync((uint)stream.Size);
                dataReader.ReadBytes(data);
            }
            await AddImageData(data);
            if (deleteAfterUsed)
                await file.DeleteAsync();
        }

        public void ShowEmoji()
        {
            if (Windows.UI.ViewManagement.InputPane.GetForCurrentView().Visible)
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().TryHide();
            else
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().TryShow();
        }

        public void AddTopic()
        {
            var index = richEditBox.Document.Selection.StartPosition;
            Text = Text.Insert(index, "##");
            richEditBox.Document.Selection.StartPosition = index + 1;
        }

        public void AddFriend()
        {
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as EmotionModel;
            var index = richEditBox.Document.Selection.StartPosition;
            Text = Text.Insert(index, item.Value);
            richEditBox.Document.Selection.StartPosition = index + item.Value.Length;
        }

        private async void AddMultipleImage(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            var files = await picker.PickMultipleFilesAsync();
            if (files != null)
                for (int i = 0; i < 9 && i < files.Count; i++)
                    await AddImageDataFromFile(files[i]);
        }

        private async void AddSingleImage(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
                await AddImageDataFromFile(file);
        }

        private async void TakePhoto(object sender, RoutedEventArgs e)
        {
            var camera = new CameraCaptureUI();
            camera.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            var file = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (file != null)
                await AddImageDataFromFile(file);
        }

        private bool _isCtrlDown;

        private void richEditBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
                _isCtrlDown = true;
            if (_isCtrlDown && e.Key == Windows.System.VirtualKey.Enter)
                PostWeibo();
        }

        private void richEditBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
                _isCtrlDown = false;
        }

        private ImageData _clickData;

        private void DeletePicture(object sender, RoutedEventArgs e)
        {
            Images.Remove(_clickData);
        }
        
        private void ChangeWeiboVisibility(object sender, RoutedEventArgs e)
        {
            var menu = sender as ToggleMenuFlyoutItem;
            foreach (var item in WeiboVisibilityFlyout.Items)
                (item as ToggleMenuFlyoutItem).IsChecked = false;
            menu.IsChecked = true;
            WeiboVisibility = (WeiboVisibility)Enum.Parse(typeof(WeiboVisibility), menu.Tag.ToString());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WeiboVisibilityIcon)));
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _clickData = (sender as FrameworkElement).DataContext as ImageData;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private async void InvertImage(object sender, RoutedEventArgs e)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget target = new CanvasRenderTarget(device, _clickData.Image.PixelWidth, _clickData.Image.PixelHeight, 96);
            using (CanvasDrawingSession session = target.CreateDrawingSession())
            using (var stream = new MemoryStream(_clickData.Data).AsRandomAccessStream())
            {
                session.Clear(Colors.Transparent);
                session.DrawImage(new InvertEffect { Source = await CanvasBitmap.LoadAsync(device, stream) as ICanvasImage });
            }
            using (var stream = new InMemoryRandomAccessStream())
            {
                await target.SaveAsync(stream, CanvasBitmapFileFormat.Jpeg);
                var bytes = new byte[stream.Size];
                await stream.AsStream().ReadAsync(bytes, 0, bytes.Length);
                var bitmap = new BitmapImage();
                stream.Seek(0);
                await bitmap.SetSourceAsync(stream);
                Images[Images.IndexOf(_clickData)].Data = bytes;
                Images[Images.IndexOf(_clickData)].Image = bitmap;
            }
        }
    }
}