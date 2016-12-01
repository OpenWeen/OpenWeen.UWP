using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Controls.Events;
using OpenWeen.UWP.Common.Extension;
using OpenWeen.UWP.Model;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OpenWeen.UWP.Common.Controls
{
    public sealed partial class WeiboTextBlock : UserControl
    {
        public event EventHandler<WeiboTopicClickEventArgs> TopicClick;

        public event EventHandler<WeiboUserClickEventArgs> UserClick;

        public WeiboTextBlock()
        {
            this.InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WeiboTextBlock), new PropertyMetadata(null, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WeiboTextBlock).TextChanged();
        }

        public bool IsTextSelectionEnabled
        {
            get { return (bool)GetValue(IsTextSelectionEnabledProperty); }
            set { SetValue(IsTextSelectionEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTextSelectionEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTextSelectionEnabledProperty =
            DependencyProperty.Register("IsTextSelectionEnabled", typeof(bool), typeof(WeiboTextBlock), new PropertyMetadata(false));



        public int MaxLines
        {
            get { return (int)GetValue(MaxLinesProperty); }
            set { SetValue(MaxLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLinesProperty =
            DependencyProperty.Register("MaxLines", typeof(int), typeof(WeiboTextBlock), new PropertyMetadata(0));



        public void TextChanged()
        {
            string text = "";
            var model = DataContext as MessageModel;
            string ortext = (model?.LongText != null) ? model.LongText.Content : Text;
            try
            {
                text = ortext.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace("\n", "<LineBreak/>");
                text = ReplaceUserName(text);
                text = ReplaceTopic(text);
                text = ReplaceHyperlink(text);
                text = ReplaceEmotion(text);
                AddBlockFromText(text);
            }
            catch
            {
                text = ortext.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace("\n", "<LineBreak/>");
                AddBlockFromText(text);
            }
        }

        private void AddBlockFromText(string text)
        {
            var xaml = string.Format(@"<Paragraph
                                        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" >
                                    <Paragraph.Inlines><Run></Run>{0}</Paragraph.Inlines>
                                    </Paragraph>", text);
            richTextBlock.Blocks.Clear();
            richTextBlock.Blocks.Add((Paragraph)XamlReader.Load(xaml));
        }

        private bool CheckIsTopic(string text) => Regex.IsMatch(text, @"#[^#]+#");

        private bool CheckIsUserName(string text) => Regex.IsMatch(text, @"@[^,，：:\s@]+");

        private string ReplaceTopic(string text)
        {
            var matches = Regex.Matches(text, @"#[^#]+#");
            foreach (Match item in matches)
            {
                text = text.Replace(item.Value, @"<InlineUIContainer><TextBlock Foreground=""{ThemeResource HyperlinkForegroundThemeBrush}"">" + item.Value + "</TextBlock></InlineUIContainer>");
            }
            return text;
        }

        private string ReplaceUserName(string text)
        {
            var matches = Regex.Matches(text, @"@[^,，：:\s@]+").Cast<Match>().Distinct((item => item.Value));
            foreach (Match item in matches)
            {
                text = text.Replace(item.Value, @"<InlineUIContainer><TextBlock Foreground=""{ThemeResource HyperlinkForegroundThemeBrush}"">" + item.Value + "</TextBlock></InlineUIContainer>");
            }
            return text;
        }

        private string ReplaceHyperlink(string text)
        {
            var matches = Regex.Matches(text, "http(s)?://([a-zA-Z|\\d]+\\.)+[a-zA-Z|\\d]+(/[a-zA-Z|\\d|\\-|\\+|_./?%=]*)?");
            var model = DataContext as MessageModel;
            var index = Math.Max(text.IndexOf("全文： http://m.weibo.cn/"), text.IndexOf("http://m.weibo.cn/client/version"));
            if (index != -1)
            {
                text = text.Remove(index);
                text += @"<InlineUIContainer><TextBlock Foreground=""{ThemeResource HyperlinkForegroundThemeBrush}"">全文</TextBlock></InlineUIContainer>";
            }
            foreach (Match item in matches)
            {
                text = text.Replace(item.Value, "<InlineUIContainer><TextBlock Foreground=\"{ThemeResource HyperlinkForegroundThemeBrush}\" Tag=\"" + item.Value + "\">网页链接</TextBlock></InlineUIContainer>");
            }
            return text;
        }

        private string ReplaceEmotion(string text)
        {
            if (string.IsNullOrEmpty(StaticResource.EmotionPattern))
                return text;
            var matches = Regex.Matches(text, StaticResource.EmotionPattern);
            foreach (Match item in matches)
            {
                text = text.Replace(item.Value, $@"<InlineUIContainer><Image Source=""{StaticResource.Emotions.FirstOrDefault(e => e.Value == item.Value).Url}"" Width=""15"" Height=""15""/></InlineUIContainer>");
            }
            return text;
        }

        private async void richTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                var text = (e.OriginalSource as TextBlock).Text;
                if (CheckIsTopic(text))
                {
                    e.Handled = true;
                    TopicClick?.Invoke(this, new WeiboTopicClickEventArgs(text.Replace("#", "")));
                }
                else if (CheckIsUserName(text))
                {
                    e.Handled = true;
                    UserClick?.Invoke(this, new WeiboUserClickEventArgs(text.Replace("@", "")));
                }
                else if((e.OriginalSource as TextBlock).Tag != null)
                {
                    e.Handled = true;
                    try
                    {
                        var item = (await Core.Api.ShortUrl.Info((e.OriginalSource as TextBlock).Tag.ToString())).Urls.FirstOrDefault();
                        switch (item.Type)
                        {
                            case 39:
                                {
                                    var picid = item.AnnotationList?.FirstOrDefault()?.Item?.PicIds?.FirstOrDefault();
                                    if (!string.IsNullOrEmpty(picid))
                                    {
                                        var items = new List<ImageModel> { new ImageModel($"http://ww1.sinaimg.cn/large/{picid}.jpg") };//TODO: better way to get image url
                                        await new ImageViewDialog(items).ShowAsync();
                                    }
                                    else if (item.AnnotationList?.FirstOrDefault()?.Item?.ObjectType == "video")
                                    {
                                        if (item.AnnotationList?.FirstOrDefault()?.Item?.OriginalUrl != null)
                                            await new WeiboVideoPlayer(item?.UrlLong, item.AnnotationList?.FirstOrDefault().Item.OriginalUrl).ShowAsync();
                                        else if (item.AnnotationList?.FirstOrDefault()?.Item?.Stream?.Url != null)
                                            await new WeiboVideoPlayer(item?.UrlLong, item.AnnotationList?.FirstOrDefault()?.Item?.Stream?.Url).ShowAsync();
                                        else
                                            await Launcher.LaunchUriAsync(new Uri(item.UrlLong));
                                    }
                                    else
                                        await Launcher.LaunchUriAsync(new Uri(item.UrlLong));
                                }
                                break;
                            default:
                                await Launcher.LaunchUriAsync(new Uri(item.UrlLong));
                                break;
                        }
                    }
                    catch
                    {
                        await Launcher.LaunchUriAsync(new Uri((e.OriginalSource as TextBlock).Tag.ToString()));
                    }
                }
            }
        }

        private string _clickedLink;

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var content = new Windows.ApplicationModel.DataTransfer.DataPackage();
            content.SetText(_clickedLink);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(content);
            _clickedLink = null;
            Notification.Show("复制成功");
        }

        private void richTextBlock_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            SetClickedLink(e.OriginalSource);
        }

        private void SetClickedLink(object originalSource)
        {
            if ((originalSource as TextBlock)?.Tag != null)
            {
                _clickedLink = (originalSource as TextBlock)?.Tag.ToString();
                menuFlyout.ShowAt(originalSource as TextBlock);
            }
        }

        private void richTextBlock_Holding(object sender, HoldingRoutedEventArgs e)
        {
            SetClickedLink(e.OriginalSource);
        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            var item = await Core.Api.ShortUrl.Info(_clickedLink);
            if (item?.Urls.FirstOrDefault()?.UrlLong == null) return;
            var content = new Windows.ApplicationModel.DataTransfer.DataPackage();
            content.SetText(item.Urls.FirstOrDefault().UrlLong);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(content);
            _clickedLink = null;
            Notification.Show("复制成功");
        }
    }
}