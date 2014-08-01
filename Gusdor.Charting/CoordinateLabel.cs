using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Gusdor.Charting
{
    public enum AxisSnapMode
    {
        XOnly,
        YOnly,
        Both
    }
    public class CoordinateLabel: ChartElementControl
    {
        #region MatchMode
        public AxisSnapMode AxisSnapMode
        {
            get { return (AxisSnapMode)GetValue(AxisSnapModeProperty); }
            set { SetValue(AxisSnapModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisSnapMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisSnapModeProperty =
            DependencyProperty.Register("AxisSnapMode", typeof(AxisSnapMode), typeof(CoordinateLabel), new FrameworkPropertyMetadata(AxisSnapMode.XOnly, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region XAxisProvider
        public AxisProvider XAxisProvider
        {
            get { return (AxisProvider)GetValue(XAxisProviderProperty); }
            set { SetValue(XAxisProviderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XAxisProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAxisProviderProperty =
            DependencyProperty.Register("XAxisProvider", typeof(AxisProvider), typeof(CoordinateLabel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)); 
        #endregion

        #region YAxisProvider
        public AxisProvider YAxisProvider
        {
            get { return (AxisProvider)GetValue(YAxisProviderProperty); }
            set { SetValue(YAxisProviderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YAxisProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YAxisProviderProperty =
            DependencyProperty.Register("YAxisProvider", typeof(AxisProvider), typeof(CoordinateLabel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region LabelFormatStringProperty
        public string LabelFormatString
        {
            get { return (string)GetValue(LabelFormatStringProperty); }
            set { SetValue(LabelFormatStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFormatString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelFormatStringProperty =
            DependencyProperty.Register("LabelFormatString", typeof(string), typeof(CoordinateLabel), new PropertyMetadata("{0:F3}, {1:F3}")); 
        #endregion

        #region MarkerRadius
        /// <summary>
        /// Gets or sets the radius of the pin highlighting the coordinates indicated.
        /// </summary>
        [System.ComponentModel.Description("Gets or sets the radius of the pin highlighting the coordinates indicated.")]
        public double MarkerRadius
        {
            get { return (double)GetValue(MarkerRadiusProperty); }
            set { SetValue(MarkerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerRadiusProperty =
            DependencyProperty.Register("MarkerRadius", typeof(double), typeof(CoordinateLabel), new PropertyMetadata(3.0)); 
        #endregion

        #region MarkerBrush
        /// <summary>
        /// Gets or sets the Brush used to fill the coordinate highlight pin.
        /// </summary>
        [System.ComponentModel.Description("Gets or sets the Brush used to fill the coordinate highlight pin.")]
        public Brush MarkerBrush
        {
            get { return (Brush)GetValue(MarkerBrushProperty); }
            set { SetValue(MarkerBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerBrushProperty =
            DependencyProperty.Register("MarkerBrush", typeof(Brush), typeof(CoordinateLabel), new PropertyMetadata(Brushes.Black)); 
        #endregion

        #region MarkerPen
        /// <summary>
        /// Gets or sets the brush used to surround the coordinate highlight pin.
        /// </summary>
        [System.ComponentModel.Description("Gets or sets the brush used to surround the coordinate highlight pin.")]
        public Pen MarkerPen
        {
            get { return (Pen)GetValue(MarkerPenProperty); }
            set { SetValue(MarkerPenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerPen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerPenProperty =
            DependencyProperty.Register("MarkerPen", typeof(Pen), typeof(CoordinateLabel), new PropertyMetadata(null)); 
        #endregion

        #region AlwaysSnapToClosest

        /// <summary>
        /// Specifies that when values exist, snapping will always occur. The closest point is chosen.
        /// </summary>
        public bool AlwaysSnapToClosest
        {
            get { return (bool)GetValue(AlwaysSnapToClosestProperty); }
            set { SetValue(AlwaysSnapToClosestProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlwaysSnapToClosest.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlwaysSnapToClosestProperty =
            DependencyProperty.Register("AlwaysSnapToClosest", typeof(bool), typeof(CoordinateLabel), new PropertyMetadata(false));
        #endregion

        #region SnapRadiusProperty
        public double SnapRadius
        {
            get { return (double)GetValue(SnapRadiusProperty); }
            set { SetValue(SnapRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SnapRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SnapRadiusProperty =
            DependencyProperty.Register("SnapRadius", typeof(double), typeof(CoordinateLabel), new PropertyMetadata(10.0)); 
        #endregion

        #region ShowLabelOnlyWhenSnappedProperty
        public bool ShowLabelOnlyWhenSnapped
        {
            get { return (bool)GetValue(ShowLabelOnlyWhenSnappedProperty); }
            set { SetValue(ShowLabelOnlyWhenSnappedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowLabelOnlyWhenSnapped.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowLabelOnlyWhenSnappedProperty =
            DependencyProperty.Register("ShowLabelOnlyWhenSnapped", typeof(bool), typeof(CoordinateLabel), new PropertyMetadata(false)); 
        #endregion

        #region ValuesProperty
        /// <summary>
        /// Gets or sets a list of values to use when SnapToValues is true.
        /// </summary>
        [System.ComponentModel.Description("Gets or sets a list of values to use when SnapToValues is true.")]
        public IEnumerable<Point?> Values
        {
            get { return (IEnumerable<Point?>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(IEnumerable<Point?>), typeof(CoordinateLabel),
            new FrameworkPropertyMetadata(new List<Point?>(), FrameworkPropertyMetadataOptions.AffectsRender, OnValuesChanged));

        static void OnValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as CoordinateLabel;
            //control.m_LineSeriesElement.Values = args.NewValue as IEnumerable<Point?>;

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
            this.InvalidateVisual();
        }
        #endregion

        #region IsMarkerShown
        /// <summary>
        /// Gets if the marker is currently being shown in any form
        /// </summary>
        public bool IsMarkerShown
        {
            get { return (bool)GetValue(IsMarkerShownProperty.DependencyProperty); }
            private set { SetValue(IsMarkerShownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMarkerShown.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey IsMarkerShownProperty =
            DependencyProperty.RegisterReadOnly("IsMarkerShown", typeof(bool), typeof(CoordinateLabel), new PropertyMetadata(false)); 
        #endregion

        #region LocationValue
        /// <summary>
        /// Gets the chart space value of the displayed position
        /// </summary>
        public Point LocationValue
        {
            get { return (Point)GetValue(LocationValuePropertyKey.DependencyProperty); }
            private set { SetValue(LocationValuePropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for LocationValue.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey LocationValuePropertyKey =
            DependencyProperty.RegisterReadOnly("LocationValue", typeof(Point), typeof(CoordinateLabel), new PropertyMetadata(new Point())); 
        #endregion

        #region LocationScreenValue
        /// <summary>
        /// Gets the screen space value of the displayed position
        /// </summary>
        public Point LocationScreenValue
        {
            get { return (Point)GetValue(LocationScreenValuePropertyKey.DependencyProperty); }
            private set { SetValue(LocationScreenValuePropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for LocationScreenValue.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey LocationScreenValuePropertyKey =
            DependencyProperty.RegisterReadOnly("LocationScreenValue", typeof(Point), typeof(CoordinateLabel), new PropertyMetadata(new Point())); 
        #endregion   

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            if (AlwaysSnapToClosest && !Values.Any()) return;

            IsMarkerShown = false;

            base.OnRender(dc);

            if (ParentChart != null)
            {
                DrawingArgs args = ParentChart.GetDrawingArgs();

                Point labelPos = System.Windows.Input.Mouse.GetPosition(ParentChart);
                Point posValue = args.Transform.Inverse.Transform(labelPos);
                Point? snapPos = GetSnapLocation(labelPos, args, dc);

                //dont render a thing if the mouse is outside the bounds of the chart
                if (!args.RenderBounds.Contains(labelPos)) return;

                if (snapPos.HasValue)
                {
                    labelPos = args.Transform.Transform(snapPos.Value);
                    posValue = snapPos.Value;
                }

                if (!(!snapPos.HasValue && ShowLabelOnlyWhenSnapped)) //show label only when snapped!
                {
                    FormattedText text = new FormattedText(GetLabel(posValue, args), System.Globalization.CultureInfo.CurrentUICulture,
                        FlowDirection.LeftToRight, new Typeface("Arial"), 10.0, Brushes.Black);
                    //dc.DrawText(text, mousePos);

                    //Draw marker
                    dc.PushClip(new RectangleGeometry(args.RenderBounds));
                    dc.DrawEllipse(MarkerBrush, MarkerPen, labelPos, MarkerRadius, MarkerRadius);
                    dc.Pop();

                    Point labelDrawPoint = GetLabelDrawPoint(new Rect(labelPos, new Size(text.Width, text.Height)), args);

                    Geometry textGeo = text.BuildGeometry(labelDrawPoint);
                    dc.DrawGeometry(null, new Pen(Brushes.White, 3), textGeo);
                    dc.DrawText(text, labelDrawPoint);

                    IsMarkerShown = true;
                }

                LocationValue = posValue;
                LocationScreenValue = labelPos;
            }
        }

        private Point GetLabelDrawPoint(Rect textRegion, DrawingArgs args)
        {
            Point shiftTopLeft = textRegion.TopLeft - new Vector(textRegion.Width + MarkerRadius, textRegion.Height + MarkerRadius);

            return new Point(Math.Max(Math.Min(shiftTopLeft.X, args.RenderBounds.Width - textRegion.Width), args.RenderBounds.Left),
                Math.Max(Math.Min(shiftTopLeft.Y, args.RenderBounds.Height - textRegion.Height), args.RenderBounds.Top));
        }

        private Point? GetSnapLocation(Point pointerPosition, DrawingArgs args, DrawingContext context)
        {
            Pen debugPen = new Pen(Brushes.Purple, 1);

            if (Math.Abs(SnapRadius) > 0)
            {
                Point transformedStartPoint = args.Transform.Inverse.Transform(pointerPosition);

                //transform bounds into chart space
                Rect transformedBounds = args.Transform.Inverse.TransformBounds(new Rect(new Size(SnapRadius, SnapRadius)));

                //create and pair each point with a vector to the cursor
                var distanceVectors = from v in Values
                                      where v.HasValue
                                      select new
                                        {
                                            Point = v.Value,
                                            VectFromCursor = transformedStartPoint - v.Value
                                        };
                if (!AlwaysSnapToClosest)
                {
                    distanceVectors = distanceVectors.Where(p => Math.Abs(p.VectFromCursor.X) < transformedBounds.X
                                                                 && Math.Abs(p.VectFromCursor.Y) < transformedBounds.Y);
#if DEBUG
                    context.PushOpacity(0.5);
                    foreach (var item in distanceVectors)
                    {
                        context.DrawEllipse(null, debugPen, args.Transform.Transform(item.Point), 10, 10);
                    }
                    context.Pop();
#endif
                }

                Point? result = null;

                //find the point with the minmum vector length to the cursor
                if (distanceVectors.Any())
                {
                    result = (from v in distanceVectors
                              let p = args.Transform.Transform(v.Point)
                              select new
                              {
                                  //TransformedPoint = p,
                                  Point = v.Point,
                                  VectFromCursor = pointerPosition - p
                              }).Aggregate((i1, i2) => 
                              {
                                  //compare vectors based upon the amtch mode
                                  switch (AxisSnapMode)
	                              {
                                      case AxisSnapMode.XOnly:
                                          return Math.Abs(i1.VectFromCursor.X) < Math.Abs(i2.VectFromCursor.X) ? i1 : i2;
                                      case AxisSnapMode.YOnly:
                                          return Math.Abs(i1.VectFromCursor.Y) < Math.Abs(i2.VectFromCursor.Y) ? i1 : i2;
                                      case AxisSnapMode.Both:
                                      default:
                                          return i1.VectFromCursor.Length < i2.VectFromCursor.Length ? i1 : i2;
	                              }
                            }).Point;
                }

                return result;
            }

            return null;
        }
        
        protected override void OnChartTransformChanged()
        {
            this.InvalidateVisual();
        }

        protected override void OnChartAttached()
        {
            base.OnChartAttached();
            ParentChart.MouseMove += oldChart_MouseMove;
            ParentChart.MouseLeave += oldChart_MouseLeave;
        }

        protected override void OnChartDettached(Chart oldChart)
        {
            base.OnChartDettached(oldChart);

            oldChart.MouseMove -= oldChart_MouseMove;
            oldChart.MouseLeave -= oldChart_MouseLeave;
        }

        void oldChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            InvalidateVisual();
        }

        void oldChart_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            InvalidateVisual();
        }

        string GetLabel(Point tooltipCoordinates, DrawingArgs args)
        {
            object x = tooltipCoordinates.X;
            object y = tooltipCoordinates.Y;

            if (XAxisProvider != null)
                x = XAxisProvider.TooltipAtPoint(tooltipCoordinates.X, new Range(args.ViewRange.Left, args.ViewRange.Right), args.Transform, new AxisDrawingArgs());

            if (YAxisProvider != null)
                y = YAxisProvider.TooltipAtPoint(tooltipCoordinates.Y, new Range(args.ViewRange.Left, args.ViewRange.Right), args.Transform, new AxisDrawingArgs());

            return string.Format(LabelFormatString, x, y);
        }
    }
}
