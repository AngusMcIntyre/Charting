using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gusdor.Charting
{
    public class GridLines:ChartElementControl
    {
        public Pen MajorTickPen
        {
            get { return (Pen)GetValue(MajorTickPenProperty); }
            set { SetValue(MajorTickPenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickPen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickPenProperty =
            DependencyProperty.Register("MajorTickPen", typeof(Pen), typeof(GridLines), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Gray), 1)));

        public Pen MinorTickPen
        {
            get { return (Pen)GetValue(MinorTickPenProperty); }
            set { SetValue(MinorTickPenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickPenProperty =
            DependencyProperty.Register("MinorTickPen", typeof(Pen), typeof(GridLines), new FrameworkPropertyMetadata(null));

        public TickList XTicks
        {
            get { return (TickList)GetValue(XTicksProperty); }
            set { SetValue(XTicksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ticks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XTicksProperty =
            DependencyProperty.Register("XTicks", typeof(TickList), typeof(GridLines), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public TickList YTicks
        {
            get { return (TickList)GetValue(YTicksProperty); }
            set { SetValue(YTicksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ticks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YTicksProperty =
            DependencyProperty.Register("YTicks", typeof(TickList), typeof(GridLines), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));    

        protected override void OnRender(DrawingContext dc)
        {

            var startTime = DateTime.Now;

            base.OnRender(dc);
            Point start, end;
            if (ParentChart == null) return;

            var args = ParentChart.GetDrawingArgs();

            dc.PushClip(new RectangleGeometry(args.RenderBounds));

            if (YTicks != null)
            {
                var major = YTicks.MajorTicks.Select(p => new { Position = p.AxisPosition, IsMajor = true });
                var minor = YTicks.MajorTicks.Select(p => new { Position = p.AxisPosition, IsMajor = false });

                foreach (var tick in major.Concat(minor))
                {
                    var pen = tick.IsMajor ? MajorTickPen : MinorTickPen;
                    if (pen == null) continue;
                    double offset = pen.Thickness / 2;

                    var pos = ParentChart.YRange.Start + ParentChart.YRange.Size * tick.Position;
                    var transformdPos = args.Transform.Transform(new Point(0, pos)).Y;
                    start = new Point(args.RenderBounds.Left, transformdPos);
                    end = new Point(args.RenderBounds.Right, transformdPos);

                    dc.PushGuidelineSet(new GuidelineSet(
                        null,
                        new[] { start.Y + offset, end.Y + offset }));
                    dc.DrawLine(pen, start, end);
                    dc.Pop();
                }
            }

            if (XTicks != null)
            {
                var major = XTicks.MajorTicks.Select(p => new { Position = p.AxisPosition, IsMajor = true });
                var minor = XTicks.MajorTicks.Select(p => new { Position = p.AxisPosition, IsMajor = false });

                foreach (var tick in major.Concat(minor))
                {
                    var pen = tick.IsMajor ? MajorTickPen : MinorTickPen;
                    if (pen == null) continue;
                    double offset = pen.Thickness / 2;

                    var pos = ParentChart.XRange.Start + ParentChart.XRange.Size * tick.Position;
                    var transformdPos = args.Transform.Transform(new Point(pos, 0)).X;
                    start = new Point(transformdPos, args.RenderBounds.Bottom);
                    end = new Point(transformdPos, args.RenderBounds.Top);

                    dc.PushGuidelineSet(new GuidelineSet(
                        new[] { start.X + offset, end.X + offset }, null));
                    dc.DrawLine(pen, start, end);
                    dc.Pop();
                }
            }
            dc.Pop(); 
                                        
            //System.Diagnostics.Debug.WriteLine(String.Format(
            //    "Gridline render took {0} ticks.", (DateTime.Now - startTime).TotalMilliseconds));  
        }

        protected override void OnChartAttached()
        {
            ParentChart.TransformChanged += ParentChart_TransformChanged;
        }

        void ParentChart_TransformChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        protected override void OnChartDettached(Chart oldChart)
        {
            if (oldChart != null)
                oldChart.TransformChanged -= ParentChart_TransformChanged;
        }


        public GridLines()
        {
            if(MajorTickPen != null)
                MajorTickPen.Freeze();

            if (MinorTickPen != null)
                MinorTickPen.Freeze();
        }
 
    }
}
