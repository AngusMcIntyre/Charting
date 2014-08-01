using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Gusdor.Charting
{
    class LineSeriesElement: IChartElement
    {
        static Pen defaultPen = new Pen(Brushes.Black, 1);

        public event EventHandler Changed;

        List<Point> m_LonePoints = new List<Point>();
        private IEnumerable<Point?> m_Values;
        private Pen m_Pen;
        private Brush m_Fill;
        /// <summary>
        /// Specifies if the stream itself has changed and needs a full redraw.
        /// </summary>
        bool m_IsDirty = true;
        StreamGeometry m_Geo = null;

        public IEnumerable<Point?> Values
        {
            get { return m_Values; }
            set { m_Values = value; m_IsDirty = true; }
        }
        public Brush Fill
        {
            get { return m_Fill; }
            set { m_Fill = value; }
        }
        public Pen Pen
        {
            get { return m_Pen; }
            set 
            { 
                m_Pen = value;
                NotifyOfChange();
            }
        }

        public LineSeriesElement(IEnumerable<Point?> values)
        {
            Pen = defaultPen;
            Values = values;
        }

        public void Draw(DrawingContext context, DrawingArgs args)
        {
            context.PushClip(new RectangleGeometry(args.RenderBounds));
            if (m_IsDirty || args.RequiresFullRedraw)
            {
                m_LonePoints.Clear();
                System.Windows.Media.StreamGeometry geo = new StreamGeometry();
                using (var c = geo.Open())
                {
                    Point? figureStart = null;
                    int pointCount = 0; //point in the figure

                    if (Values.Count() == 1 && Values.First().HasValue)
                    {
                        m_LonePoints.Add(Values.First().Value);
                    }
                    else
                    {
                        foreach (var p in Values)
                        {
                            if (p.HasValue)
                            {
                                if (!figureStart.HasValue)
                                {
                                    figureStart = p.Value;
                                    c.BeginFigure(figureStart.Value, false, false);
                                    pointCount = 1;
                                }
                                else
                                {
                                    c.LineTo(p.Value, true, true);
                                    pointCount++;
                                }
                            }
                            else
                            {
                                //detect lone points and draw a cross
                                if (pointCount == 1)
                                {
                                    m_LonePoints.Add(figureStart.Value);
                                }
                                figureStart = null;
                            }
                        }
                    }
                }

                m_Geo = geo;
                m_IsDirty = false;
            }

            m_Geo.Transform = args.Transform;
            context.DrawGeometry(null, Pen, m_Geo);

            var radius = Pen.Thickness;
            foreach (var p in m_LonePoints)
            {
                context.DrawEllipse(m_Pen.Brush, null, args.Transform.Transform(p), radius, radius);
            }
        }

        private static void DrawCross(StreamGeometryContext c, Point point)
        {
            c.BeginFigure(new Point(point.X - 2, point.Y - 2), false, false);
            c.LineTo(new Point(point.X + 2, point.Y + 2), true, false);

            c.BeginFigure(new Point(point.X - 2, point.Y + 2), false, false);
            c.LineTo(new Point(point.X + 2, point.Y - 2), true, false);
        }

        public void NotifyOfChange()
        {
            if(Changed != null)
                Changed(this, EventArgs.Empty);
        }
    }
}
