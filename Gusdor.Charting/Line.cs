using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Gusdor.Charting
{
    [System.Windows.Markup.ContentProperty("Position")]
    public class Line : ChartElementControl
    {
        public Pen LinePen
        {
            get { return (Pen)GetValue(LinePenProperty); }
            set { SetValue(LinePenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinePen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinePenProperty =
            DependencyProperty.Register("LinePen", typeof(Pen), typeof(Line),
            new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Black), 1), FrameworkPropertyMetadataOptions.AffectsRender));
      
        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double), typeof(Line), 
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));


        public System.Windows.Controls.Orientation Orientation
        {
            get { return (System.Windows.Controls.Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(System.Windows.Controls.Orientation), typeof(Line), 
            new FrameworkPropertyMetadata(System.Windows.Controls.Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender));

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (ParentChart == null) return;

            double lineWidth = 1.0;

            var args = ParentChart.GetDrawingArgs();

            drawingContext.PushClip(new RectangleGeometry(args.RenderBounds));


            Point start, end;
            if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                var transformdPos = args.Transform.Transform(new Point(0, Position)).Y;

                start = new Point(args.RenderBounds.Left, transformdPos);
                end = new Point(args.RenderBounds.Right, transformdPos);

                var offset = lineWidth / 2;
                drawingContext.PushGuidelineSet(new GuidelineSet(
                    null,
                    new[] { start.Y + offset, end.Y + offset }));
            }
            else
            {
                var transformdPos = args.Transform.Transform(new Point(Position, 0)).X;
                start = new Point(transformdPos, args.RenderBounds.Top);
                end = new Point(transformdPos, args.RenderBounds.Bottom);

                var offset = lineWidth / 2;
                drawingContext.PushGuidelineSet(new GuidelineSet(
                    new[] { start.X + offset, end.X + offset },
                    null));
            }

            drawingContext.DrawLine(this.LinePen, start, end);
       
            drawingContext.Pop();
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
            if (oldChart != null)
            {
                oldChart.SizeChanged -= ParentChart_SizeChanged;
                oldChart.TransformChanged -= ParentChart_TransformChanged;
            }
        }
    }
}
