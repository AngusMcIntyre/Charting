using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Gusdor.Charting
{
    /// <summary>
    ///Basic chart.
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ChartElementContainer))]
    public class Chart: ItemsControl
    {
        #region TransformProperty
        public event EventHandler TransformChanged;

        public Transform Transform
        {
            get { return (Transform)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transform.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform), typeof(Chart), 
            new FrameworkPropertyMetadata(new MatrixTransform(), System.Windows.FrameworkPropertyMetadataOptions.AffectsRender, OnTransformChanged, OnCoerceTransform));

        static void OnTransformChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var chart = sender as Chart;
            if (chart.IsLoaded)// && !chart.m_SettingTransform)
            {
                if (!chart.m_SettingViewRegion)
                {
                    chart.m_SettingTransform = true;
                    sender.SetCurrentValue(ViewAreaProperty, chart.GetViewRange((Transform)args.NewValue));//Set the view are
                    chart.m_SettingTransform = false;
                }

                chart.SetViewRanges();
                //var va = chart.ViewArea;
                //chart.XRange = new Range(va.X, va.X + va.Width);
                //chart.YRange = new Range(va.Y, va.Y + va.Height);
            }

            if (chart.TransformChanged != null)
                chart.TransformChanged(chart, EventArgs.Empty);
        }

        static object OnCoerceTransform(DependencyObject sender, object value)
        {
            Chart chart = sender as Chart;
            Transform original = chart.Transform;
            Transform trans = value as Transform;

            //fail if there is no inversion for this value.
            if (trans.Inverse == null)
                return original;

            //do not allow empty view regions
            Rect tester = trans.TransformBounds(new Rect(new Size(100, 100)));
            if (tester.IsEmpty || tester.Width == 0 || tester.Height == 0)
                return original;

            //TODO - too big!?
            //TODO - too small
            return value;
        }
        #endregion

        #region ViewAreaProperty
        /// <summary>
        /// Gets or sets the charts viewable area.
        /// </summary>
        public System.Windows.Rect ViewArea
        {
            get { return (System.Windows.Rect)GetValue(ViewAreaProperty); }
            set { SetValue(ViewAreaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewAreaProperty =
            DependencyProperty.Register("ViewArea", typeof(System.Windows.Rect), typeof(Chart), new PropertyMetadata(new Rect(new Point(0,0), new Size(100,100)), OnViewAreaChanged));

        static void OnViewAreaChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var chart = sender as Chart;

            Rect newRange = (Rect)e.NewValue;
            Rect oldRange = (Rect)e.OldValue;

            if (!chart.IsLoaded)
            {
                chart.m_OnLoadViewRegion = newRange; //defer until load
            }
            else if(!chart.m_SettingTransform)
            {
                chart.SetViewRegion(newRange, oldRange, true);
            }          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newRange"></param>
        /// <param name="oldRange"></param>
        /// <param name="offsetFromSize">If true, new newRegion is treated as an offset to the current view, otherwise the new newRegion is offset from the chart size</param>
        private void SetViewRegion(Rect newRange, Rect oldRange, bool offsetFromSize)
        {
            this.m_SettingViewRegion = true;

            if (this.ActualHeight == 0 || this.ActualWidth == 0)
                offsetFromSize = false;

            if ((oldRange.Width == 0 && oldRange.Height == 0) || oldRange.IsEmpty || offsetFromSize)
            {
                oldRange = new System.Windows.Rect(new Size(this.ActualWidth, this.ActualHeight));
            }

            var scale = new ScaleTransform(
                Math.Max(0.00001, oldRange.Width / newRange.Width),
                Math.Max(0.00001, oldRange.Height / newRange.Height));

            //use the reverse vector to get the scale to centre correctly on the new newRegion
            //invert y  to take flipped coordinate system into account
            var translate = new TranslateTransform(oldRange.X - newRange.X, -(oldRange.Y - newRange.Y));

            //I can only speculate as to why these transforms are in the correct order.
            //Figuring it out was trial und error.
            var grp = new TransformGroup();
            grp.Children.Add(translate);

            if(!offsetFromSize)
                grp.Children.Add(this.Transform);

            grp.Children.Add(scale);

            this.SetCurrentValue(TransformProperty, grp);

            this.m_SettingViewRegion = false;
        }         
        #endregion

        #region XRangeProperty
        public Range XRange
        {
            get { return (Range)GetValue(XRangePropertyKey.DependencyProperty); }
            private set { SetValue(XRangePropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for XRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey XRangePropertyKey =
            DependencyProperty.RegisterReadOnly("XRange", typeof(Range), typeof(Chart), new PropertyMetadata(new Range())); 
        #endregion

        #region YRangeProperty
        public Range YRange
        {
            get { return (Range)GetValue(YRangePropertyKey.DependencyProperty); }
            private set { SetValue(YRangePropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for YRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey YRangePropertyKey =
            DependencyProperty.RegisterReadOnly("YRange", typeof(Range), typeof(Chart), new PropertyMetadata(new Range())); 
        #endregion

        #region ScaleOnResizeProperty
        /// <summary>
        /// Gets or sets if the viewable newRegion will be rescaled when the chart control is resized.
        /// </summary>
        public bool ScaleOnResize
        {
            get { return (bool)GetValue(ScaleOnResizeProperty); }
            set { SetValue(ScaleOnResizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleOnResize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleOnResizeProperty =
            DependencyProperty.Register("ScaleOnResize", typeof(bool), typeof(Chart), new PropertyMetadata(true)); 
        #endregion

        Rect? m_OnLoadViewRegion = null;    //view newRegion to view on load
        object dirtyLock = new object();
        bool m_SettingTransform = false;
        bool m_SettingViewRegion = false;

        static Chart()
        {
            ItemsControl.ItemsPanelProperty.OverrideMetadata(typeof(Chart), new System.Windows.FrameworkPropertyMetadata(new ItemsPanelTemplate(new System.Windows.FrameworkElementFactory(typeof(Grid)))));
        }
        public Chart()
        {
            //this.ItemsPanel = new ItemsPanelTemplate(new System.Windows.FrameworkElementFactory(typeof(Grid)));
            this.Loaded += new System.Windows.RoutedEventHandler(Chart_Loaded);
            this.MinWidth = 1;
            this.MinHeight = 1;
        }

        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new ChartElementContainer() { Chart = this };
        }
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
        }

        void SetViewRanges()
        {
            if (this.Transform.Inverse != null)
            {
                var origin = this.GetFlippedTransform().Inverse.Transform(new Point(0, 0));
                var max = this.GetFlippedTransform().Inverse.Transform(new Point(this.ActualWidth, this.ActualHeight));

                this.XRange.Start = origin.X;
                this.XRange.End = max.X;

                this.YRange.Start = max.Y;
                this.YRange.End = origin.Y;
            }
        }

        internal DrawingArgs GetDrawingArgs()
        {
            var trans = this.GetFlippedTransform();
            return new DrawingArgs(trans, GetViewRange(trans), new System.Windows.Rect(new Size(this.ActualWidth, this.ActualHeight)));
        }
        Rect GetViewRange(Transform transform)
        {
            var vr = new Rect(new Point(), new Point(this.ActualWidth, this.ActualHeight));
            vr = this.FlipTransform(transform).Inverse.TransformBounds(vr);

            //var res = new Rect(new Point(XRange.Start, YRange.Start), new Size(XRange.Size, YRange.Size));


            return vr;
        }
        /// <summary>
        /// Gets a transform that flips the Y axis to a more traditional space.
        /// </summary>
        /// <returns></returns>
        Transform GetFlippedTransform()
        {
            //var scale = new ScaleTransform(1, -1);
            //var trans = new TranslateTransform(0, this.ActualHeight);
            //var comp = new TransformGroup();
            //comp.Children.Add(scale);
            //comp.Children.Add(Transform);
            //comp.Children.Add(trans);   //post transform with a translate to give a transformed offset for the inversion scale.

            //return new MatrixTransform(comp.Value);

            return FlipTransform(Transform);
        }

        /// <summary>
        /// Inverts Y axis and returns result
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        Transform FlipTransform(Transform transform)
        {
            var scale = new ScaleTransform(1, -1);
            var trans = new TranslateTransform(0, this.ActualHeight);
            var comp = new TransformGroup();
            comp.Children.Add(scale);
            comp.Children.Add(transform);
            comp.Children.Add(trans);   //post transform with a translate to give a transformed offset for the inversion scale.

            return new MatrixTransform(comp.Value);
        }

        protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            OnSizeChanged(sizeInfo);
        }

        bool actuallyLoaded = true;
        private void OnSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
        {
            if (this.IsLoaded && actuallyLoaded)
            {
                if (sizeInfo.PreviousSize.Width != 0 && sizeInfo.PreviousSize.Height != 0)
                {
                    if (this.ScaleOnResize)
                    {
                        var scale = new ScaleTransform(
                            Math.Max(0.00001, sizeInfo.NewSize.Width / sizeInfo.PreviousSize.Width),
                            Math.Max(0.00001, sizeInfo.NewSize.Height / sizeInfo.PreviousSize.Height), 0, 0);

                        var grp = new TransformGroup();
                        grp.Children.Add(Transform);
                        grp.Children.Add(scale);

                        this.SetViewRanges();

                        this.m_SettingTransform = true;
                        this.SetCurrentValue(TransformProperty, grp);
                        this.m_SettingTransform = false;

                        this.SetViewRanges();
                    }
                    else
                    {
                        this.m_SettingViewRegion = true;
                        this.SetCurrentValue(ViewAreaProperty, this.GetViewRange((Transform)this.Transform));//Set the view area
                        this.m_SettingViewRegion = false;

                        this.SetViewRanges();
                    }
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            this.Focus();
        }

        public new void Focus()
        {
            base.Focus();
        }

        void Chart_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            actuallyLoaded = true;
            if (this.ActualHeight > 0 && this.ActualWidth > 0)
            {
                SetViewRanges();

                //if (m_OnLoadViewRegion.HasValue)
                //    this.SetViewRegion(m_OnLoadViewRegion.Value, new Rect(new Size(this.ActualWidth, this.ActualHeight)), true);
                //else
                //{
                //    m_SettingTransform = true;
                //    this.SetCurrentValue(ViewAreaProperty, GetViewRange(this.Transform));
                //    m_SettingTransform = false;
                //}
            }
        }
    }

    /// <summary>
    /// Container class for chart elements.
    /// </summary>
    public class ChartElementContainer : ContentControl
    {
        public Chart Chart
        {
            get { return (Chart)GetValue(ChartProperty); }
            set { SetValue(ChartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Chart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartProperty =
            DependencyProperty.Register("Chart", typeof(Chart), typeof(ChartElementContainer), new PropertyMetadata(null));
    }
}
