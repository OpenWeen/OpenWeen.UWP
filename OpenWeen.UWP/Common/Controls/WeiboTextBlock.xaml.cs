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
                text = ortext.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&apos;").Replace("\"", "&quot;");
                text = ReplaceUserName(text);
                text = ReplaceTopic(text);
                text = ReplaceHyperlink(text);
                text = ReplaceEmotion(text);
                AddBlockFromText(text);
            }
            catch
            {
                text = ortext.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&apos;").Replace("\"", "&quot;");
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
                text = text.Replace(item.Value, @"<InlineUIContainer><TextBlock Foreground=""{ThemeResource HyperlinkForegroundThemeBrush}""><Underline>" + item.Value + "</Underline></TextBlock></InlineUIContainer>");
            }
            return text;
        }

        private string ReplaceUserName(string text)
        {
            var matches = Regex.Matches(text, @"@[^,，：:\s@]+").Cast<Match>().Distinct((item => item.Value));
            foreach (Match item in matches)
            {
                text = text.Replace(item.Value, @"<InlineUIContainer><TextBlock Foreground=""{ThemeResource HyperlinkForegroundThemeBrush}""><Underline>" + item.Value + "</Underline></TextBlock></InlineUIContainer>");
            }
            return text;
        }

        private string ReplaceHyperlink(string text)
        {
            var matches = Regex.Matches(text, "http(s)?://([a-zA-Z|\\d]+\\.)+[a-zA-Z|\\d]+(/[a-zA-Z|\\d|\\-|\\+|_./?%=]*)?");
            //foreach (Match item in matches)
            //{
            //    var model = DataContext as BaseModel;
            //    string innerText = null;
            //    if (model != null)
            //    {
            //        innerText = model.UrlStruct?.Where(m => m.ShortUrl == item.Value)?.FirstOrDefault()?.UrlTitle;
            //        var memodel = DataContext as MessageModel;
            //        if (memodel != null && string.IsNullOrEmpty(innerText))
            //        {
            //            innerText = memodel.RetweetedStatus?.UrlStruct?.Where(m => m.ShortUrl == item.Value)?.FirstOrDefault()?.UrlTitle;
            //        }
            //    }
            //    innerText = string.IsNullOrEmpty(innerText) ? "网页链接" : innerText;
            //    text = text.Replace(item.Value, $@"<Hyperlink NavigateUri=""{item.Value}"">{innerText}</Hyperlink>");
            //}
            var model = DataContext as MessageModel;
            var index = text.IndexOf("全文： http://m.weibo.cn/");
            if (index != -1)
            {
                text = text.Remove(index);
                text += @"<InlineUIContainer><TextBlock Foreground=""{ThemeResource HyperlinkForegroundThemeBrush}""><Underline>全文</Underline></TextBlock></InlineUIContainer>";
            }
            //if (model?.IsLongText == true)
            //{
            //    var a = text.IndexOf("全文");
            //    a = text.IndexOf("全文：");
            //    text = text.Remove(text.IndexOf("全文") - 1);
            //    //text = text.Replace($"全文： {matches[matches.Count - 1].Value}", @"<InlineUIContainer><TextBlock Foreground=""{ThemeResource HyperlinkForegroundThemeBrush}""><Underline>全文</Underline></TextBlock></InlineUIContainer>");
            //}
            foreach (Match item in matches)
            {
                text = text.Replace(item.Value, "<InlineUIContainer><TextBlock Foreground=\"{ThemeResource HyperlinkForegroundThemeBrush}\" Tag=\"" + item.Value + "\"><Underline>网页链接</Underline></TextBlock></InlineUIContainer>");
            }
            return text;
        }

        private string ReplaceEmotion(string text)
        {
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
                                            throw new NotSupportedException();
                                    }
                                    else
                                        throw new NotSupportedException();
                                }
                                break;
                            default:
                                throw new NotSupportedException();
                        }
                    }
                    catch
                    {
                        await Launcher.LaunchUriAsync(new Uri((e.OriginalSource as TextBlock).Tag.ToString()));
                    }
                }
            }
        }
    }
}