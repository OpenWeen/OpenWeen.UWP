using System;
using System.Windows.Input;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace OpenWeen.UWP.Common.Controls
{
    public class AdaptiveGridView : ItemsControl
    {
        /// <summary>
        /// Identifies the <see cref="ItemClickCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register(nameof(ItemClickCommand), typeof(ICommand), typeof(AdaptiveGridView), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the <see cref="OneRowModeEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OneRowModeEnabledProperty =
            DependencyProperty.Register(nameof(OneRowModeEnabled), typeof(bool), typeof(AdaptiveGridView), new PropertyMetadata(false, (o, e) => { OnOneRowModeEnabledChanged(o, e.NewValue); }));

        /// <summary>
        /// Identifies the <see cref="ItemWidth"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the <see cref="DesiredWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DesiredWidthProperty =
            DependencyProperty.Register(nameof(DesiredWidth), typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(double.NaN, DesiredWidthChanged));

        /// <summary>
        /// Identifies the <see cref="StretchContentForSingleRow"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchContentForSingleRowProperty =
        DependencyProperty.Register(nameof(StretchContentForSingleRow), typeof(bool), typeof(AdaptiveGridView), new PropertyMetadata(true, OnStretchContentForSingleRowPropertyChanged));

        private static void OnOneRowModeEnabledChanged(DependencyObject d, object newValue)
        {
            var self = d as AdaptiveGridView;
            self.DetermineOneRowMode();
        }

        private static void DesiredWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as AdaptiveGridView;
            self.RecalculateLayout(self.ActualWidth);
        }

        private static void OnStretchContentForSingleRowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as AdaptiveGridView;
            self.RecalculateLayout(self.ActualWidth);
        }
        
        public double DesiredWidth
        {
            get { return (double)GetValue(DesiredWidthProperty); }
            set { SetValue(DesiredWidthProperty, value); }
        }

        public bool StretchContentForSingleRow
        {
            get { return (bool)GetValue(StretchContentForSingleRowProperty); }
            set { SetValue(StretchContentForSingleRowProperty, value); }
        }

        public ICommand ItemClickCommand
        {
            get { return (ICommand)GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }
        
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }
        
        public bool OneRowModeEnabled
        {
            get { return (bool)GetValue(OneRowModeEnabledProperty); }
            set { SetValue(OneRowModeEnabledProperty, value); }
        }
        

        private double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        private static int CalculateColumns(double containerWidth, double itemWidth)
        {
            var columns = (int)Math.Round(containerWidth / itemWidth);
            if (columns == 0)
            {
                columns = 1;
            }

            return columns;
        }



        private bool _isLoaded;
        
        public AdaptiveGridView()
        {
            IsTabStop = false;
            SizeChanged += OnSizeChanged;
            //ItemClick += OnItemClick;
            Items.VectorChanged += ItemsOnVectorChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }
        
        protected override void PrepareContainerForItemOverride(DependencyObject obj, object item)
        {
            base.PrepareContainerForItemOverride(obj, item);
            var element = obj as FrameworkElement;
            if (element != null)
            {
                var heightBinding = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath("ItemHeight"),
                    Mode = BindingMode.TwoWay
                };

                var widthBinding = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath("ItemWidth"),
                    Mode = BindingMode.TwoWay
                };

                element.SetBinding(HeightProperty, heightBinding);
                element.SetBinding(WidthProperty, widthBinding);
            }
        }

        protected virtual double CalculateItemWidth(double containerWidth)
        {
            double desiredWidth = double.IsNaN(DesiredWidth) ? containerWidth : DesiredWidth;

            var columns = CalculateColumns(containerWidth, desiredWidth);
            
            if (Items != null && Items.Count > 0 && Items.Count < columns && StretchContentForSingleRow)
            {
                columns = Items.Count;
            }

            return (containerWidth / columns) - 5;
        }
        
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            OnOneRowModeEnabledChanged(this, OneRowModeEnabled);
        }

        private void ItemsOnVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            if (!double.IsNaN(ActualWidth))
            {
                RecalculateLayout(ActualWidth);
            }
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var cmd = ItemClickCommand;
            if (cmd != null)
            {
                if (cmd.CanExecute(e.ClickedItem))
                {
                    cmd.Execute(e.ClickedItem);
                }
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width != e.NewSize.Width)
            {
                RecalculateLayout(e.NewSize.Width);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            DetermineOneRowMode();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;
        }

        private void DetermineOneRowMode()
        {
            if (_isLoaded)
            {
                var itemsWrapGridPanel = ItemsPanelRoot as ItemsWrapGrid;

                if (OneRowModeEnabled)
                {
                    var b = new Binding()
                    {
                        Source = this,
                        Path = new PropertyPath("ItemHeight")
                    };

                    if (itemsWrapGridPanel != null)
                    {
                        itemsWrapGridPanel.Orientation = Orientation.Vertical;
                    }

                    this.SetBinding(MaxHeightProperty, b);

                    ScrollViewer.SetVerticalScrollMode(this, ScrollMode.Disabled);
                    ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
                    ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Visible);
                    ScrollViewer.SetHorizontalScrollMode(this, ScrollMode.Enabled);
                }
                else
                {
                    this.ClearValue(MaxHeightProperty);
                    if (itemsWrapGridPanel != null)
                    {
                        itemsWrapGridPanel.Orientation = Orientation.Horizontal;
                    }

                    ScrollViewer.SetVerticalScrollMode(this, ScrollMode.Enabled);
                    ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Visible);
                    ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
                    ScrollViewer.SetHorizontalScrollMode(this, ScrollMode.Disabled);
                }
            }
        }

        private void RecalculateLayout(double containerWidth)
        {
            if (containerWidth > 0)
            {
                ItemWidth = CalculateItemWidth(containerWidth);
            }
        }
    }
}
