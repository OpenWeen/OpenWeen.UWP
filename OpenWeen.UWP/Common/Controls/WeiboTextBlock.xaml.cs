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
        private const string AT = "@[^,\uff0c\uff1a:\\s@]+";
        private const string TOPIC = "#[^#]+#";
        private const string EMOJI = "\\[[\\w]+\\]";
        private const string URL = "http://[-a-zA-Z0-9+&@#/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#/%=~_|]";
        private readonly string REGEX = $"({AT})|({TOPIC})|({EMOJI})|({URL})";

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
            text = ortext.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace("\n", "<LineBreak/>");
            var index = Math.Max(text.IndexOf("全文： http://m.weibo.cn/"), text.IndexOf("http://m.weibo.cn/client/version"));
            if (index != -1)
            {
                text = text.Remove(index);
                text += @"<InlineUIContainer><TextBlock Foreground=""{ThemeResource HyperlinkForegroundThemeBrush}"">全文</TextBlock></InlineUIContainer>";
            }
            var matches = Regex.Matches(text, REGEX).Cast<Match>().Distinct((item => item.Value));
            foreach (var item in matches)
            {
                var at = item.Groups[1];
                var topic = item.Groups[2];
                var emoji = item.Groups[3];
                var url = item.Groups[4];
                if (at.Success)
                {
                    text = text.Replace(at.Value, @"<InlineUIContainer><TextBlock Foreground=""{ThemeResource HyperlinkForegroundThemeBrush}"">" + at.Value + "</TextBlock></InlineUIContainer>");
                }
                if (topic.Success)
                {
                    text = text.Replace(topic.Value, @"<InlineUIContainer><TextBlock Foreground=""{ThemeResource HyperlinkForegroundThemeBrush}"">" + topic.Value + "</TextBlock></InlineUIContainer>");
                }
                if (emoji.Success && StaticResource.Emotions?.Any(e => e.Value == emoji.Value) == true)
                {
                    text = text.Replace(emoji.Value, $@"<InlineUIContainer><Image Source=""{StaticResource.Emotions.FirstOrDefault(e => e.Value == emoji.Value).Url}"" Width=""15"" Height=""15""/></InlineUIContainer>");
                }
                if (url.Success)
                {
                    text = text.Replace(url.Value, "<InlineUIContainer><TextBlock Foreground=\"{ThemeResource HyperlinkForegroundThemeBrush}\" Tag=\"" + url.Value + "\">网页链接</TextBlock></InlineUIContainer>");
                }
            }
            try
            {
                AddBlockFromText(text);
            }
            catch (Exception)
            {
                AddBlockFromText(ortext.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace("\n", "<LineBreak/>"));
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

        private bool CheckIsTopic(string text) => Regex.IsMatch(text, TOPIC);

        private bool CheckIsUserName(string text) => Regex.IsMatch(text, AT);
        
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
                        var item = (await Core.Api.ShortUrl.InfoByWeico(StaticResource.Uid.ToString(), (e.OriginalSource as TextBlock).Tag.ToString())).urls.FirstOrDefault();
                        switch (item.type)
                        {
                            case 39:
                                {
                                    var picid = item.annotations?.FirstOrDefault()?._object?.pic_ids?.FirstOrDefault();
                                    if (!string.IsNullOrEmpty(picid))
                                    {
                                        var items = new List<ImageModel> { new ImageModel($"http://ww1.sinaimg.cn/large/{picid}.jpg") };//TODO: better way to get image url
                                        await new ImageViewDialog(items).ShowAsync();
                                    }
                                    else if (item.annotations?.FirstOrDefault()?.object_type == "video")
                                    {
                                        if (item.annotations?.FirstOrDefault()?._object?.stream?.hd_url != null)
                                            await new WeiboVideoPlayer(item?.url_long, item.annotations?.FirstOrDefault()?._object?.stream?.hd_url).ShowAsync();
                                        else if (item.annotations?.FirstOrDefault()?._object?.stream?.url != null)
                                            await new WeiboVideoPlayer(item?.url_long, item.annotations?.FirstOrDefault()?._object?.stream?.url).ShowAsync();
                                        else
                                            await Launcher.LaunchUriAsync(new Uri(item.url_long));
                                    }
                                    else
                                        await Launcher.LaunchUriAsync(new Uri(item.url_long));
                                }
                                break;
                            default:
                                await Launcher.LaunchUriAsync(new Uri(item.url_long));
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