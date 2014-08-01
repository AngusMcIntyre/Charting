using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Gusdor.Charting
{
    /// <summary>
    /// Implementation of IChartElement to assist with debugging.
    /// </summary>
    public class ViewRangeOverlay: ChartElementControl
    {
        protected override void OnRender(System.Windows.Media.DrawingContext context)
        {
#if DEBUG
            if (ParentChart != null)
            {
                var args = ParentChart.GetDrawingArgs();
                var debugPen = new Pen(new SolidColorBrush(new Color { A = 100, R = 255 }), 1);
                var regionPen = new Pen(new SolidColorBrush(new Color { A = 100, G = 255, B = 255 }), 1);

                //var viewRangeFixed = new ScaleTransform(1, -1).TransformBounds(args.ViewRange);
                var viewRangeFixed = new System.Windows.Rect(args.ViewRange.X, -args.ViewRange.Y, args.ViewRange.Width, args.ViewRange.Height);

                context.DrawText(new FormattedText(string.Format("Viewable range {0} - {1}", args.ViewRange.TopLeft, args.ViewRange.BottomRight),
                    System.Globalization.CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Black),
                    new System.Windows.Point(5, 5));

                double offset = debugPen.Thickness / 2;
                GuidelineSet guides = new GuidelineSet(
                    new[] 
                { 
                    args.RenderBounds.Left + offset, args.RenderBounds.Right + offset,
                    viewRangeFixed.Left + offset, viewRangeFixed.Right + offset
                },
                    new[] 
                {
                    args.RenderBounds.Top + offset, args.RenderBounds.Bottom + offset,
                    viewRangeFixed.Top + offset, viewRangeFixed.Bottom + offset
                });

                context.PushGuidelineSet(guides);
                context.DrawRectangle(null, regionPen, args.RenderBounds);
                context.DrawRectangle(null, debugPen, viewRangeFixed);
                context.Pop();
            }
#endif
        }

        protected override void OnChartAttached()
        {
            ParentChart.TransformChanged += ParentChart_TransformChanged;
            ParentChart.SizeChanged += ParentChart_SizeChanged;
        }

        void ParentChart_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        void ParentChart_TransformChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        protected override void OnChartDettached(Chart oldChart)
        {
            ParentChart.TransformChanged -= ParentChart_TransformChanged;
            ParentChart.SizeChanged -= ParentChart_SizeChanged;
        }
    }
}
