using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using anim = System.Windows.Media.Animation;

namespace Gusdor.Charting
{
    public class ChartRegionZoomBehavior : System.Windows.Interactivity.Behavior<Chart>
    {
        Point? m_DragStart = null;
        ZoomRegionAdorner m_Adorner = null;
        AdornerLayer m_Layer = null;
        int m_RunningAnimations = 0;

        #region RegionBorderProperty
        public Pen RegionBorder
        {
            get { return (Pen)GetValue(RegionBorderProperty); }
            set { SetValue(RegionBorderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegionBorderProperty =
            DependencyProperty.Register("RegionBorder", typeof(Pen), typeof(ChartRegionZoomBehavior), new PropertyMetadata(new Pen(Brushes.Black, 1), OnRegionBorderChanged));

        static void OnRegionBorderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as ChartRegionZoomBehavior;

            item.m_Adorner.RegionBorder = (Pen)e.NewValue;
        } 
        #endregion

        #region RegionFillProperty
        public Brush RegionFill
        {
            get { return (Brush)GetValue(RegionFillProperty); }
            set { SetValue(RegionFillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegionFillProperty =
            DependencyProperty.Register("RegionFill", typeof(Brush), typeof(ChartRegionZoomBehavior), new PropertyMetadata(null, OnRegionFillChanged));

        static void OnRegionFillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as ChartRegionZoomBehavior;

            item.m_Adorner.RegionFill = (Brush)e.NewValue;
        } 
        #endregion

        #region AnimateProperty
        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Animate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimateProperty =
            DependencyProperty.Register("Animate", typeof(bool), typeof(ChartRegionZoomBehavior), new PropertyMetadata(true)); 
        #endregion

        #region AnimationDurationProperty
        public Duration AnimationDuration
        {
            get { return (Duration)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnimationDuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(ChartRegionZoomBehavior),
            new PropertyMetadata(new Duration(new TimeSpan(0, 0, 1)))); 
        #endregion   
    
        /// <summary>
        /// Gets or sets the Easing function applied to zooming animations.
        /// </summary>
        public anim.IEasingFunction EasingFunction
        {
            get { return (anim.IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EasingFunction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EasingFunctionProperty = 
            DependencyProperty.Register("EasingFunction", typeof(anim.IEasingFunction), typeof(ChartRegionZoomBehavior), new PropertyMetadata(null));
       
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.MouseDown += AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseUp += AssociatedObject_MouseLeftButtonUp;
            this.AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            this.AssociatedObject.TransformChanged += AssociatedObject_TransformChanged;

            var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(AssociatedObject);

            if (layer == null)
            {
                //defer to load
                AssociatedObject.Dispatcher.BeginInvoke((Action)AddAdorner, System.Windows.Threading.DispatcherPriority.Loaded);
            }
            else
            {
                AddAdorner(layer);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (m_Layer != null)
                m_Layer.Remove(m_Adorner);

            m_Adorner = null;
            m_Layer = null;

            this.AssociatedObject.MouseDown -= AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseUp -= AssociatedObject_MouseLeftButtonUp;
            this.AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            this.AssociatedObject.TransformChanged -= AssociatedObject_TransformChanged;
        }
        /// <summary>
        /// Attached adorner to layer
        /// </summary>
        /// <param name="layer"></param>
        void AddAdorner(AdornerLayer layer)
        {
            m_Layer = layer;
            layer.Add(m_Adorner = new ZoomRegionAdorner(AssociatedObject));
        }
        void AddAdorner()
        {
            var layer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(AssociatedObject);
            AddAdorner(layer);
        }

        void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (m_DragStart.HasValue)
            {
                var pos = e.GetPosition(this.AssociatedObject);

                if (m_Adorner != null)
                    m_Adorner.ZoomRegion = new Rect(m_DragStart.Value, pos);
            }
        }

        void AssociatedObject_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Right)
            {
                var args = AssociatedObject.GetDrawingArgs();

                //tranform may not always be appropriate
                if (args.Transform.Inverse != null)
                {
                    var newViewRegion = args.Transform.TransformBounds(m_Adorner.ZoomRegion);

                    //transform zoom newRegion to chart space
                    newViewRegion = args.Transform.Inverse.TransformBounds(m_Adorner.ZoomRegion);

                    //set new view newRegion
                    SetViewRegion(newViewRegion);
                }

                System.Windows.Input.Mouse.Capture(null);
                m_DragStart = null;
                m_Adorner.ZoomRegion = Rect.Empty;
                this.AssociatedObject.Cursor = null;
            }
        }

        /// <summary>
        /// Sets the view region, animates the change if required
        /// </summary>
        /// <param name="newRegion"></param>
        void SetViewRegion(Rect newRegion)
        {
            if (newRegion.IsEmpty) return;

            if (Animate)
            {
                var timeline = new anim.RectAnimation(newRegion, this.AnimationDuration, anim.FillBehavior.Stop);
                timeline.EasingFunction = this.EasingFunction;

                timeline.Completed += timeline_Completed;
                ++m_RunningAnimations;
                AssociatedObject.BeginAnimation(Chart.ViewAreaProperty, timeline, anim.HandoffBehavior.SnapshotAndReplace);
            }
            else
            {
                var oldRegion = AssociatedObject.ViewArea;
                AssociatedObject.ViewArea = newRegion;
            }
        }

        void timeline_Completed(object sender, EventArgs e)
        {
            //unsubcribe to prevent leaky behavior
            var timeline = (sender as anim.AnimationClock).Timeline as anim.RectAnimation;
            //timeline.Completed -= timeline_Completed; this is frozen?

            //this removes all animation values - only do this if all animation are done.
            //REMEMBER - the user can drag and release a region WHILST an animation is playing.
            if (--m_RunningAnimations == 0)
            {
                //dont know why this doesn't override binding - it might later? :D
                AssociatedObject.ViewArea = timeline.To.Value;
                //AssociatedObject.SetCurrentValue(Chart.ViewAreaProperty, timeline.To.Value);

                //remove all animation values.
                AssociatedObject.BeginAnimation(Chart.ViewAreaProperty, null);
            }
        }

        void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Right)
            {
                m_DragStart = e.GetPosition(this.AssociatedObject);
                System.Windows.Input.Mouse.Capture(this.AssociatedObject);
            }
        }

        void AssociatedObject_TransformChanged(object sender, EventArgs e)
        {
            if (m_Adorner != null)
                m_Adorner.InvalidateVisual();
        }
    }
    /// <summary>
    /// Renders the zoom region as drawn by the user.
    /// </summary>
    internal class ZoomRegionAdorner : System.Windows.Documents.Adorner
    {
        Chart m_Chart = null;
        /// <summary>
        /// Gets or sets the zoom newRegion to draw.
        /// </summary>
        public Rect ZoomRegion
        {
            get { return (Rect)GetValue(ZoomRegionProperty); }
            set { SetValue(ZoomRegionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomRegion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomRegionProperty =
            DependencyProperty.Register("ZoomRegion", typeof(Rect), typeof(ZoomRegionAdorner), new FrameworkPropertyMetadata(Rect.Empty, 
                FrameworkPropertyMetadataOptions.AffectsRender));

        public Pen RegionBorder
        {
            get { return (Pen)GetValue(RegionBorderProperty); }
            set { SetValue(RegionBorderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegionBorderProperty =
            DependencyProperty.Register("RegionBorder", typeof(Pen), typeof(ZoomRegionAdorner), 
            new FrameworkPropertyMetadata(new Pen(Brushes.Black, 1), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush RegionFill
        {
            get { return (Brush)GetValue(RegionFillProperty); }
            set { SetValue(RegionFillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegionFillProperty =
            DependencyProperty.Register("RegionFill", typeof(Brush), typeof(ZoomRegionAdorner), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public ZoomRegionAdorner(Chart chart):base(chart)
        {
            m_Chart = chart;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.PushClip(new RectangleGeometry(new Rect(AdornedElement.RenderSize)));
            if (ZoomRegion != Rect.Empty)
            {
                var gl = new GuidelineSet(
                    new[] { ZoomRegion.Left + RegionBorder.Thickness / 2, ZoomRegion.Right + RegionBorder.Thickness / 2 },
                    new[] { ZoomRegion.Top + RegionBorder.Thickness / 2, ZoomRegion.Bottom + RegionBorder.Thickness / 2 });

                drawingContext.PushGuidelineSet(gl);
                drawingContext.DrawRectangle(RegionFill, RegionBorder, ZoomRegion);
                drawingContext.Pop();
            }

            drawingContext.Pop();
        }
    }
}
