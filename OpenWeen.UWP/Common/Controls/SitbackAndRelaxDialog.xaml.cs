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
using PropertyChanged;
using System.ComponentModel;

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace OpenWeen.UWP.Common.Controls
{
    [ImplementPropertyChanged]
    public sealed partial class SitbackAndRelaxDialog : ContentDialog
    {   
        public double ProgressValue { get; set; } = 0d;
        public double ProgressMaximum { get; set; } = 100d;
        public bool IsIndeterminate { get; set; } = true;
        public string DialogText { get; set; } = "坐和放宽，这不需要太久";
        public SitbackAndRelaxDialog()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
