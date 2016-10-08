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
using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common;
using OpenWeen.UWP.Common.Entities;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OpenWeen.UWP.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PostWeiboPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<ImageData> Images { get; } = new ObservableCollection<ImageData>();
        private IPostWeibo _data;
        public bool AllowMorePicture { get; set; } = true;
        private bool _isSending;

        public int TextCount
        {
            get
            {
                string value = "";
                
                richEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.NoHidden, out value);
                return 140 - Encoding.GetEncoding("gb2312").GetByteCount(value) / 2;
            }
        }

        public string Text
        {
            get
            {
                string value = "";
                richEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.NoHidden, out value);
                return value;
            }
            set
            {
                richEditBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, value);
            }
        }

        public List<IGrouping<string, EmotionModel>> Emojis => StaticResource.Emotions?.GroupBy(item => string.IsNullOrEmpty(item.Category) ? "表情" : item.Category).ToList();

        public PostWeiboPage()
        {
            this.InitializeComponent();
            Transitions = new TransitionCollection { new NavigationThemeTransition() { DefaultNavigationTransitionInfo = new SlideNavigationTransitionInfo() } };
            if (StaticResource.Emotions != null)
            {
                cvs.Source = Emojis;
                (semanticZoom.ZoomedOutView as ListViewBase).ItemsSource = cvs.View.CollectionGroups;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!(e.Parameter is IPostWeibo))
                throw new ArgumentException("parameter must be IPostWeibo");
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
                    {
                        richEditBox.Document.Selection.StartPosition = 0;
                    }
                    else
                    {
                        richEditBox.Document.Selection.StartPosition = Text.Length;
                    }
                }
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
        }

        public class ImageData
        {
            public BitmapImage Image { get; set; }
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
                dialog.Commands.Add(new UICommand("删除超出部分并发送", (command)=> 
                {
                    Text = Text.Remove(139);
                    PostWeibo();
                }));
                dialog.Commands.Add(new UICommand("取消"));
                await dialog.ShowAsync();
                return;
            }
            _isSending = true;
            var sardialog = new Common.Controls.SitbackAndRelaxDialog();
            sardialog.ShowAsync();
            bool isSuccess = false;
            switch (_data.Type)
            {
                case PostWeiboType.NewPost:
                    isSuccess = await NewPost();
                    break;

                case PostWeiboType.RePost:
                    isSuccess = await RePost();
                    break;

                case PostWeiboType.Comment:
                    isSuccess = await Comment();
                    break;

                default:
                    break;
            }
            sardialog.Hide();
            if (isSuccess)
            {
                Frame.GoBack();
            }
            else
            {
                Common.Controls.Notification.Show("发送失败");
            }
            _isSending = false;
        }

        private async Task<bool> Comment()
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
                    return true;
                }
                catch (Exception e) when (e is WebException || e is System.Net.Http.HttpRequestException) 
                {
                    return false;
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
                    return true;

                }
                catch (Exception e) when (e is WebException || e is System.Net.Http.HttpRequestException)
                {
                    return false;
                }
            }
            return false;
        }

        private async Task<bool> RePost()
        {
            var data = _data as RepostData;
            try
            {
                if (Images.Count > 0)
                    await Core.Api.Statuses.PostWeibo.RepostWithPic(data.ID, Text, new MediaModel((await Core.Api.Statuses.PostWeibo.UploadPicture(Images.FirstOrDefault().Data)).PicID));
                else
                    await Core.Api.Statuses.PostWeibo.Repost(data.ID, Text);
                return true;

            }
            catch (Exception e) when (e is WebException || e is System.Net.Http.HttpRequestException)
            {
                return false;
            }
        }

        private async Task<bool> NewPost()
        {
            try
            {
                if (Images.Count > 0)
                {
                    var pics = new List<PictureModel>();
                    foreach (var item in Images)
                    {
                        pics.Add(await Core.Api.Statuses.PostWeibo.UploadPicture(item.Data));
                    }
                    await Core.Api.Statuses.PostWeibo.PostWithMultiPics(Text?.Length > 0 ? Text : "分享图片", string.Join(",", pics.Select(item => item.PicID)));
                    return true;
                }
                else if (Text?.Length > 0)
                {
                    await Core.Api.Statuses.PostWeibo.Post(Text);
                    return true;
                }
            }
            catch (Exception e) when (e is WebException || e is System.Net.Http.HttpRequestException)
            {

            }
            return false;
        }
        
        private async void richEditBox_DragOver(object sender, DragEventArgs e)
        {
            var def = e.GetDeferral();
            e.Handled = true;
            if (e.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Bitmap))
            {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            }
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
            {
                await AddImageDataFromFile(await GetFileFromBitmap(await e.DataView.GetBitmapAsync()));
            }
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
            {
                using (DataReader dataReader = new DataReader(stream))
                {
                    data = new byte[stream.Size];
                    await dataReader.LoadAsync((uint)stream.Size);
                    dataReader.ReadBytes(data);
                }
            }
            await AddImageData(data);
            if (deleteAfterUsed)
            {
                await file.DeleteAsync();
            }
        }

        public void ShowEmoji()
        {
            if (Windows.UI.ViewManagement.InputPane.GetForCurrentView().Visible)
            {
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().TryHide();
            }
            else
            {
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().TryShow();
            }
        }

        public void AddImage(object sender, object e)
        {
            var menu = Resources["PictureFlyout"] as MenuFlyout;
            menu.ShowAt(sender as FrameworkElement);
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
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            var files = await picker.PickMultipleFilesAsync();
            if (files != null)
            {
                for (int i = 0; i < 9 && i < files.Count; i++)
                {
                    await AddImageDataFromFile(files[i]);
                }
            }
        }

        private async void AddSingleImage(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await AddImageDataFromFile(file);
            }
        }

        private async void TakePhoto(object sender, RoutedEventArgs e)
        {
            var camera = new CameraCaptureUI();
            camera.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            var file = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (file != null)
            {
                await AddImageDataFromFile(file);
            }
        }

        private bool _isCtrlDown;

        private void richEditBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
            {
                _isCtrlDown = true;
            };
            if (_isCtrlDown && e.Key == Windows.System.VirtualKey.Enter)
            {
                PostWeibo();
            }
        }

        private void richEditBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
            {
                _isCtrlDown = false;
            }
        }

        private ImageData _clickData;
        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _clickData = (sender as Image).DataContext as ImageData;
            var menu = Resources["ImageTapFlyout"] as MenuFlyout;
            menu.ShowAt(sender as FrameworkElement);
        }

        private void DeletePicture(object sender, RoutedEventArgs e)
        {
            Images.Remove(_clickData);
        }
    }
}