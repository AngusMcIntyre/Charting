using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Gusdor.Charting
{
    class HorizontalLine: IChartElement
    {
        /// <summary>
        /// Chart space position for this line - the Y axis value
        /// </summary>
        public double Position { get; set; }
        /// <summary>
        /// Gets or sets the colour of the line
        /// </summary>
        public System.Windows.Media.Color Color { get; set; }

        public event EventHandler Changed;

        public void Draw(System.Windows.Media.DrawingContext context, DrawingArgs args)
        {
            double lineWidth = 1.0;

            context.PushClip(new RectangleGeometry(args.RenderBounds));

            var transformdPos = args.Transform.Transform(new Point(0, Position)).Y;

            Point start = new Point(args.RenderBounds.Left, transformdPos);
            Point end = new Point(args.RenderBounds.Right, transformdPos);

            var offset = lineWidth / 2;
            context.PushGuidelineSet(new GuidelineSet(
                new[] { start.X + offset, end.X + offset },
                new[] { start.Y + offset, end.Y + offset }));

            context.DrawLine(new Pen(new SolidColorBrush(Color), lineWidth), start, end);

            context.Pop();
        }
    }
}