using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using OpenWeen.Core.Model;
using OpenWeen.Core.Model.Status;
using OpenWeen.UWP.Common.Controls.Events;
using OpenWeen.UWP.Common.Extension;
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



        public void TextChanged()
        {
            string text = "";
            var model = DataContext as MessageModel;
            string ortext = (model != null && model.IsLongText && model.LongText != null) ? model.LongText.Content : Text;
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
                                    <Paragraph.Inlines>
                                    <Run></Run>
                                      {0}
                                    </Paragraph.Inlines>
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
            foreach (Match item in matches)
            {
                var model = DataContext as BaseModel;
                string innerText = null;
                if (model != null)
                {
                    innerText = model.UrlStruct?.Where(m => m.ShortUrl == item.Value)?.FirstOrDefault()?.UrlTitle;
                    var memodel = DataContext as MessageModel;
                    if (memodel != null && string.IsNullOrEmpty(innerText))
                    {
                        innerText = memodel.RetweetedStatus?.UrlStruct?.Where(m => m.ShortUrl == item.Value)?.FirstOrDefault()?.UrlTitle;
                    }
                }
                innerText = string.IsNullOrEmpty(innerText) ? "网页链接" : innerText;
                text = text.Replace(item.Value, $@"<Hyperlink NavigateUri=""{item.Value}"">{innerText}</Hyperlink>");
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

        private void richTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
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
            }
        }
    }
}