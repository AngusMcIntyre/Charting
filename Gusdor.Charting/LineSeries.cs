using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Gusdor.Charting
{
    [System.Windows.Markup.ContentProperty("Values")]
    public class LineSeries: ChartElementControl
    {
        #region ProximityCullingAggressionProperty
        /// <summary>
        /// Gets or sets how aggresive the proximity culling is for this line series, where 2 is twice as aggressive as 1.
        /// </summary>
        /// <remarks>This value is a multiplier applied to the vector representation of the chart space range of one screen space pixel. 
        /// Points closer than this vector to the last drawn point in a block are not drawn.</remarks>
        public double ProximityCullingAggression
        {
            get { return (double)GetValue(ProximityCullingAggressionProperty); }
            set { SetValue(ProximityCullingAggressionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProximityCullingAggression.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProximityCullingAggressionProperty =
            DependencyProperty.Register("ProximityCullingAggression", typeof(double), typeof(LineSeries), new PropertyMetadata(3.0)); 
        #endregion

        #region LineTranslateProperty
        /// <summary>
        /// Gets or sets a transform to translate the series independantly of the viewport.
        /// </summary>
        public TranslateTransform LineTranslate
        {
            get { return (TranslateTransform)GetValue(LineTranslateProperty); }
            set { SetValue(LineTranslateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineTranslate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineTranslateProperty =
            DependencyProperty.Register("LineTranslate", typeof(TranslateTransform), typeof(LineSeries),
            new FrameworkPropertyMetadata(new TranslateTransform(), FrameworkPropertyMetadataOptions.AffectsRender)); 
        #endregion

        #region LineScaleProperty
        /// <summary>
        /// Gets or sets a transform to scale the series independantly of the viewport.
        /// </summary>
        public ScaleTransform LineScale
        {
            get { return (ScaleTransform)GetValue(LineScaleProperty); }
            set { SetValue(LineScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineScaleProperty =
            DependencyProperty.Register("LineScale", typeof(ScaleTransform), typeof(LineSeries), 
            new FrameworkPropertyMetadata(new ScaleTransform(), FrameworkPropertyMetadataOptions.AffectsRender)); 
        #endregion

        #region LinePenProperty
        public Pen LinePen
        {
            get { return (Pen)GetValue(LinePenProperty); }
            set { SetValue(LinePenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinePen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinePenProperty =
            DependencyProperty.Register("LinePen", typeof(Pen), typeof(LineSeries),
            new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Black), 1), FrameworkPropertyMetadataOptions.AffectsRender)); 
        #endregion

        #region ValuesProperty
        public IEnumerable<Point?> Values
        {
            get { return (IEnumerable<Point?>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(IEnumerable<Point?>), typeof(LineSeries),
            new FrameworkPropertyMetadata(new List<Point?>(), FrameworkPropertyMetadataOptions.AffectsRender, OnValuesChanged));

        static void OnValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as LineSeries;
            //control.m_LineSeriesElement.Values = args.NewValue as IEnumerable<Point?>;
            control.m_IsDirty = true;

            if (args.NewValue is System.Collections.Specialized.INotifyCollectionChanged)
            {
                (args.NewValue as System.Collections.Specialized.INotifyCollectionChanged).CollectionChanged += control.LineSeries_CollectionChanged;
            }
            if (args.OldValue is System.Collections.Specialized.INotifyCollectionChanged)
            {
                (args.OldValue as System.Collections.Specialized.INotifyCollectionChanged).CollectionChanged -= control.LineSeries_CollectionChanged;
            }
        }

        void LineSeries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.m_IsDirty = true;
            this.InvalidateVisual();
        } 
        #endregion

        #region FillOrientation
        /// <summary>
        /// Specifies the orientation of the fill when a fill brush is assigned.
        /// </summary>
        /// <remarks>When horizontal, the fill is between the series and the Fill origin on the X axis.
        /// When vertical, the fill is between the series and the fill origin on the Y axis.</remarks>
        [System.ComponentModel.Description("Specifies the orientation of the fill when a fill brush is assigned.")]
        public Orientation FillOrientation
        {
            get { return (Orientation)GetValue(FillOrientationProperty); }
            set { SetValue(FillOrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillOrientationProperty =
            DependencyProperty.Register("FillOrientation", typeof(Orientation), typeof(LineSeries), new PropertyMetadata(Orientation.Horizontal)); 
        #endregion

        #region FillOrigin
        /// <summary>
        /// Gets or sets the value within the range to which the fill will originate from.
        /// </summary>
        public double FillOrigin
        {
            get { return (double)GetValue(FillOriginProperty); }
            set { SetValue(FillOriginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillOrigin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillOriginProperty =
            DependencyProperty.Register("FillOrigin", typeof(double), typeof(LineSeries), new PropertyMetadata(0.0)); 
        #endregion

        #region FillBrush
        public Brush FillBrush
        {
            get { return (Brush)GetValue(FillBrushProperty); }
            set { SetValue(FillBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillBrushProperty =
            DependencyProperty.Register("FillBrush", typeof(Brush), typeof(LineSeries), new PropertyMetadata(null)); 
        #endregion

        //Vector? m_SmallestProximityPass = null;
        //Vector? m_LargestProximityFailure = null;
        Vector? m_LastPixelValueVect = null;
        List<Point> m_LonePoints = new List<Point>();
        bool m_IsDirty = false;
        //LineSeriesElement m_LineSeriesElement = new LineSeriesElement(new Point?[]{});
        StreamGeometry m_Geo = null;

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {

            long startTime = Environment.TickCount;

            base.OnRender(drawingContext);

            //m_LineSeriesElement.Fill = this.Background;
            //m_LineSeriesElement.Pen = this.LinePen;

            if (ParentChart != null)
            {
                var args = ParentChart.GetDrawingArgs();
                //drawingContext.DrawRectangle(new SolidColorBrush(Colors.Red), null, args.RenderBounds);

                //m_LineSeriesElement.Draw(drawingContext, args);
                Draw(drawingContext, args);
            } 

            //System.Diagnostics.Debug.WriteLine(String.Format(
            //    "Lineseries render took {0} ticks.", Environment.TickCount - startTime));
   
        }

        void Draw(DrawingContext context, DrawingArgs args)
        {
            context.PushClip(new RectangleGeometry(args.RenderBounds));

            TransformGroup grp = new TransformGroup();
            grp.Children.Add(LineScale);
            grp.Children.Add(LineTranslate);
            context.PushTransform(grp);

            //find the value (chart space) range of 1 logical pixel
            //use inverse because when zooming in (scaling up), the value range of the a pixel becomes smaller
            var logicalPixRange = args.Transform.Inverse.TransformBounds(new Rect(new Size(1,1)));
            var logicalPixVect = new Vector(logicalPixRange.Width, logicalPixRange.Height);

            //var bounds = args.Transform.Inverse.TransformBounds(args.RenderBounds);
            //logicalPixVect = new Vector(bounds.Width / args.RenderBounds.Width, bounds.Height / args.RenderBounds.Height);


            bool pixValueChanged = !logicalPixVect.Equals(m_LastPixelValueVect);
            m_LastPixelValueVect = logicalPixVect;

            if (ProximityCullingAggression > 0 && pixValueChanged)
            {
                //if (m_LargestProximityFailure.HasValue
                //    && m_LargestProximityFailure.Value.Length > logicalPixVect.Length / ProximityCullingAggression)
                //    m_IsDirty = true;

                //if (m_SmallestProximityPass.HasValue
                //    && m_SmallestProximityPass.Value.Length < logicalPixVect.Length * ProximityCullingAggression) 
                    m_IsDirty = true;
            }

            if (m_IsDirty)
            {
                //reset as new proximities will be calculated
                //m_LargestProximityFailure = null;
                //m_SmallestProximityPass = null;

                m_LonePoints.Clear();
                System.Windows.Media.StreamGeometry geo = new StreamGeometry();
                using (var c = geo.Open())
                {
                    int proxFailCount = 0; //count of points that failed proximity checking
                    int drawnPoints = 0; //total points drawn.

                    Point? figureStart = null;
                    int pointCount = 0; //point in the figure

                    if (Values.Count() == 1 && Values.First().HasValue)
                    {
                        m_LonePoints.Add(Values.First().Value);
                    }
                    else
                    {
                        Point? lastPoint = null;
                        var vals = Values.ToArray();
                        for (int i = 0; i < vals.Length; i++)
                        {
                            var p = vals[i];

                            if (p.HasValue)
                            {
                                if (!figureStart.HasValue)
                                {
                                    lastPoint = figureStart = p.Value;
                                    c.BeginFigure(GetFillOrigin(figureStart.Value, args), true, false);
                                    c.LineTo(p.Value, false, true);
                                    pointCount = 1;
                                    drawnPoints++;

                                    //Draw bounding boxes for culling.

                                    //context.DrawRectangle(null, new Pen(Brushes.Plum, 1), new Rect(args.Transform.Transform(figureStart.Value), 
                                    //    new Size(logicalPixRange.Size.Width * ProximityCullingAggression, logicalPixRange.Size.Height * ProximityCullingAggression)));

                                    //context.DrawEllipse(null, new Pen(Brushes.Plum, 1),
                                    //    args.Transform.Transform(figureStart.Value), 
                                    //    logicalPixVect.Length * ProximityCullingAggression, logicalPixVect.Length * ProximityCullingAggression);
                                }
                                else
                                {
                                    bool lastPointInBlock = i == vals.Length-1 || vals[i+1] == null;

                                    //perform proximity cull
                                    var vectFromLast = (lastPoint.Value - p.Value);
                                    if (!(lastPoint.HasValue
                                        && ProximityCullingAggression > 0
                                        //&& vectFromLast.Length < logicalPixVect.Length)
                                        && Math.Abs(vectFromLast.X) < Math.Abs(logicalPixVect.X * ProximityCullingAggression)
                                        && Math.Abs(vectFromLast.Y) < Math.Abs(logicalPixVect.Y * ProximityCullingAggression))
                                        || lastPointInBlock)
                                    {
                                        lastPoint = p.Value;
                                        c.LineTo(p.Value, true, true);
                                        pointCount++;
                                        drawnPoints++;

                                        //Store SMALLEST proximity that PASSES for retesting on next draw
                                        //if (!m_SmallestProximityPass.HasValue || vectFromLast.Length < m_SmallestProximityPass.Value.Length)
                                        //    m_SmallestProximityPass = vectFromLast;
                                    }
                                    else
                                    {
                                        //Store LARGEST proximity that FAILS for retesting on next draw
                                        //if (!m_LargestProximityFailure.HasValue || vectFromLast.Length > m_LargestProximityFailure.Value.Length)
                                        //    m_LargestProximityFailure = vectFromLast;
                                        proxFailCount++;
                                    }
                                }
                            }
                            else
                            {
                                //detect lone points and draw a cross
                                if (pointCount == 1)
                                {
                                    m_LonePoints.Add(figureStart.Value);
                                }
                                else
                                    c.LineTo(GetFillOrigin(lastPoint.Value, args), false, false);

                                lastPoint = figureStart = null;
                            }
                        }

                        if(lastPoint != null)
                            c.LineTo(GetFillOrigin(lastPoint.Value, args), false, false);
                    }

                    //System.Threading.ThreadPool.QueueUserWorkItem(p =>
                    //    {
                    //        System.Diagnostics.Debug.WriteLine("Culled: " + proxFailCount);
                    //        System.Diagnostics.Debug.WriteLine("Passed: " + drawnPoints);
                    //    });
                }

                m_Geo = geo;
                m_IsDirty = false;
            }

            m_Geo.Transform = args.Transform;
            context.DrawGeometry(FillBrush, LinePen, m_Geo);           
            var radius = LinePen.Thickness;
            foreach (var p in m_LonePoints)
            {
                context.DrawEllipse(FillBrush, null, args.Transform.Transform(p), radius/2, radius/2);
            }

            //foreach (var p in Values)
            //{
            //    if (p.HasValue)
            //        context.DrawRectangle(Brushes.Purple, null, new Rect(args.Transform.Transform(p.Value), new Size(2, 2)));
            //}
        }

        Point GetFillOrigin(Point sourceValue, DrawingArgs args)
        {
            //switch (FillOrientation)
            //{
            //    default:
            //    case Orientation.Vertical:
            //        return new Point(args.Transform.Transform(new Point(FillOrigin, 0)).X, sourceValue.Y);
            //    case Orientation.Horizontal:
            //        return new Point(sourceValue.X, args.Transform.Transform(new Point(0, FillOrigin)).Y);
            //}

            switch (FillOrientation)
            {
                default:
                case Orientation.Vertical:
                    return new Point(FillOrigin, sourceValue.Y);
                case Orientation.Horizontal:
                    return new Point(sourceValue.X, FillOrigin);
            }
        }

        public LineSeries()
        {

        }
        public LineSeries(IEnumerable<Point?> values)
        {
            this.Values = values;
        }

        protected override void OnChartAttached()
        {
            ParentChart.SizeChanged += ParentChart_SizeChanged;
            ParentChart.TransformChanged += ParentChart_TransformChanged;
        }

        void ParentChart_TransformChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        void ParentChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        protected override void OnChartDettached(Chart oldChart)
        {
            oldChart.SizeChanged -= ParentChart_SizeChanged;
            oldChart.TransformChanged -= ParentChart_TransformChanged;
        }
    }
}
