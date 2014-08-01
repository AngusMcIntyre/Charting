using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Gusdor.Charting
{
    /// <summary>
    /// Host class for drawn ticks
    /// </summary>
    public class TickHost: System.Windows.Controls.Control
    {
        public Axis ParentAxis
        {
            get { return (Axis)GetValue(ParentAxisProperty); }
            set { SetValue(ParentAxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentAxisProperty =
            DependencyProperty.Register("ParentAxis", typeof(Axis), typeof(TickHost), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            DrawMethod(drawingContext);

            base.OnRender(drawingContext);
        }

        void DrawMethod(System.Windows.Media.DrawingContext drawingContext)
        {
            DrawAxisLine(drawingContext, ParentAxis.LinePen);
            DrawTicks(drawingContext);
        }

        /// <summary>
        /// Draws the dividing line seen between the chart area and the axis ticks
        /// </summary>
        /// <param name="drawingContext">Drawing Context to push to.</param>
        /// <param name="linePen">Pen to draw with.</param>
        private void DrawAxisLine(DrawingContext drawingContext, Pen linePen)
        {
            //Create guideline set for the axis lines in all positions
            var glset = new GuidelineSet();
            glset.GuidelinesX.Add(0.0 + (linePen.Thickness / 2));
            glset.GuidelinesX.Add(this.ActualWidth + (linePen.Thickness / 2));
            glset.GuidelinesY.Add(0.0 + (linePen.Thickness / 2));
            glset.GuidelinesY.Add(this.ActualHeight + (linePen.Thickness / 2));
            drawingContext.PushGuidelineSet(glset);

            //Draw the top line - this is always present
            switch (ParentAxis.Orientation)
            {
                case System.Windows.Controls.Orientation.Horizontal:
                    {
                        if (ParentAxis.FlipLabel)
                        {
                            drawingContext.DrawLine(linePen, new Point(0, this.ActualHeight), new Point(this.ActualWidth, this.ActualHeight));
                        }
                        else
                        {
                            drawingContext.DrawLine(linePen, new Point(0, 0), new Point(this.ActualWidth, 0));
                        }
                        break;
                    }
                case System.Windows.Controls.Orientation.Vertical:
                    {
                        if (ParentAxis.FlipLabel)
                        {
                            drawingContext.DrawLine(linePen, new Point(0, 0), new Point(0, this.ActualHeight));
                        }
                        else
                            drawingContext.DrawLine(linePen, new Point(this.ActualWidth, 0), new Point(this.ActualWidth, this.ActualHeight));
                        break;
                    }
            }

            drawingContext.Pop();
        }

        private void DrawTicks(DrawingContext drawingContext)
        {
            //Get drawing Args
            AxisDrawingArgs args = ParentAxis.GetDrawingArgs();

            Pen linePen = new Pen(Brushes.Black, 1);
            TickList ticks = ParentAxis.Ticks;

            DrawAxisLine(drawingContext, linePen);

            //Draw Major Ticks
            double largestTickSize = DrawData(drawingContext, linePen, ticks.MajorTicks, true, ParentAxis.MajorTickLength, args);

            if (args.MinorTickCount > 0)
            {
                //Draw Minor Ticks
                largestTickSize = Math.Max(largestTickSize,
                          DrawData(drawingContext, linePen, ticks.MinorTicks, false, ParentAxis.MinorTickLength, args));
            }

            //Set the required axis width
            //double requiredAxisSize = largestTickSize + axisLabelSize + this.LabelPadding;
            double requiredAxisSize = largestTickSize + ParentAxis.LabelPadding;

            if (ParentAxis.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                if (requiredAxisSize != this.ActualWidth)
                {
                    this.Width = requiredAxisSize;
                    this.Height = double.NaN;
                    //AnimateWidth(requiredAxisSize);
                }
            }
            else
            {
                if (requiredAxisSize != this.ActualHeight)
                {
                    this.Height = requiredAxisSize;
                    this.Width = double.NaN;
                    //AnimateHeight(requiredAxisSize);
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Draws the given list of ticks to the axis, based upon orientation
        /// </summary>
        /// <param name="drawingContext">Drawing Context to push to</param>
        /// <param name="linePen">Pen to draw lines with</param>
        /// <param name="a_Ticks">The List of ticks to display</param>
        /// <param name="a_DrawValues">Boolean flag, dictating whether labels should be drawn</param>
        /// <param name="a_TickLength">The length of the draw ticks</param>
        /// <returns>The minimun size required to display the drawn ticks and labels</returns>
        private double DrawData(DrawingContext drawingContext, Pen linePen, List<TickList.Tick> a_Ticks, bool a_DrawValues, double a_TickLength, AxisDrawingArgs args)
        {
            double largestTickSize = 0.0;
            double previousTickEnd = 0.0;
            double labelStart = 0.0;
            //double labelEnd = 0.0;

            //If this is a Y axis, flip the coordinate system.
            if (ParentAxis.Orientation == System.Windows.Controls.Orientation.Vertical)
                drawingContext.PushTransform(new ScaleTransform(1, -1, 0, this.ActualHeight / 2));

            int counter = 1;
            foreach (TickList.Tick currentTick in a_Ticks)
            {
                if (currentTick.AxisPosition <= 1.0 && currentTick.AxisPosition >= 0.0)
                {
                    Point tickLocation = DrawTick(currentTick, drawingContext, linePen, a_TickLength);

                    //push guidelines
                    if (this.ParentAxis != null)
                    {
                        var gl = new GuidelineSet();
                        switch (this.ParentAxis.Orientation)
                        {
                            case Orientation.Horizontal:
                                gl.GuidelinesX.Add(tickLocation.X + linePen.Thickness /2);
                                break;
                            case Orientation.Vertical:
                                gl.GuidelinesY.Add(tickLocation.Y + linePen.Thickness / 2);
                                break;
                        }
                        drawingContext.PushGuidelineSet(gl);
                    }

#if DEBUG
                    //Debug brush
                    SolidColorBrush brush = Brushes.Green.Clone();
                    brush.Opacity = 0.5;
#endif

                    //If labels are required
                    if (a_DrawValues)
                    {
                        //#if DEBUG
                        //                    FormattedText currentLabel = m_AxisCalculator.CreateLabel(counter + ": " + currentTick.Value.ToString(), m_DrawOptions);
                        //                    currentLabel.SetForegroundBrush(Brushes.Red, 0, 2);
                        //#else
                        FormattedText currentLabel = ParentAxis.AxisProvider.CreateLabel(currentTick.Value.ToString(), args);
   
                        //#endif
                        Point labelLocation = new Point();

                        switch (ParentAxis.Orientation)
                        {
                            case System.Windows.Controls.Orientation.Horizontal:
                                //Set the alignment to centre on an X Axis
                                currentLabel.TextAlignment = TextAlignment.Center;

                                labelLocation = new Point(tickLocation.X, tickLocation.Y);

                                //Set the label start location
                                labelStart = tickLocation.X - currentLabel.Width;

                                if (labelStart > previousTickEnd || previousTickEnd == 0)
                                {
                                    //Set the end point for the next label.
                                    previousTickEnd = tickLocation.X + Axis.MinLabelGap + (currentLabel.Width);

                                    #region Debug
                                    //Draw debug border
                                    //drawingContext.DrawRectangle(brush, null, new Rect(new Point(labelStart, labelLocation.Y), new Size(currentLabel.Width * 2, currentLabel.Height)));
                                    #endregion

                                    drawingContext.DrawText(currentLabel, labelLocation);

                                    //Set the largest tick size
                                    largestTickSize = Math.Max(currentLabel.Height + a_TickLength, largestTickSize);
                                }
                                break;

                            case System.Windows.Controls.Orientation.Vertical:
                                //Set Alignment to Right on Y Axis
                                currentLabel.TextAlignment = this.ParentAxis.FlipLabel ? TextAlignment.Right : TextAlignment.Left;

                                //Set the label start location
                                labelStart = tickLocation.Y - currentLabel.Height / 2;

                                if (labelStart > previousTickEnd || previousTickEnd == 0)
                                {
                                    labelLocation = new Point(tickLocation.X, tickLocation.Y);
                                    labelLocation.Y -= currentLabel.Height / 2;

                                    currentLabel.MaxTextWidth = this.ActualWidth;

                                    if (ParentAxis.FlipLabel)
                                    {
                                        labelLocation.X += 2;
                                    }
                                    else
                                        labelLocation.X -= 2 + currentLabel.Width;


                                    //Flip the coordinate system to prevent the text from being upside down.
                                    drawingContext.PushTransform(new ScaleTransform(1, -1, 0, tickLocation.Y));
                                    drawingContext.DrawText(currentLabel, labelLocation);
                                    drawingContext.Pop();

                                    //Set the largest tick size
                                    largestTickSize = Math.Max(currentLabel.MinWidth + a_TickLength + 2, largestTickSize);

                                    //Set the end point for the next label.
                                    previousTickEnd = tickLocation.Y + Axis.MinLabelGap + (currentLabel.Height / 2);
                                }
                                break;
                        }
                        counter++;
                    }
                }
            }
            if (ParentAxis.Orientation == System.Windows.Controls.Orientation.Vertical)
                drawingContext.Pop();   //Axis inversion

            return largestTickSize;
        }

        /// <summary>
        /// Draws a tick to a DrawingContext
        /// </summary>
        /// <param name="a_TickToDraw">The TickList.Tick to draw.</param>
        /// <param name="drawingContext">The DrawingContext to draw to.</param>
        /// <param name="a_Pen">The pen to draw with.</param>
        /// <param name="a_Length">The Length of the tick.</param>
        /// <returns></returns>
        private Point DrawTick(TickList.Tick a_TickToDraw, DrawingContext drawingContext, Pen a_Pen, double a_Length)
        {
            GuidelineSet gl = new GuidelineSet();
            Point tickStart = new Point();
            Point tickEnd = new Point();

            if (a_TickToDraw.AxisPosition <= 1.0 && a_TickToDraw.AxisPosition >= 0.0)
            {
                switch (ParentAxis.Orientation)
                {
                    case System.Windows.Controls.Orientation.Horizontal:
                        tickStart.X = a_TickToDraw.AxisPosition * this.ActualWidth;

                        tickEnd.X = tickStart.X;
                        tickEnd.Y = a_Length;

                        gl.GuidelinesX.Add(tickStart.X + (a_Pen.Thickness / 2));

                        break;

                    case System.Windows.Controls.Orientation.Vertical:
                        if (ParentAxis.FlipLabel)
                        {
                            //Use these coords for alternative position (on the right of the chart)
                            tickStart.Y = a_TickToDraw.AxisPosition * this.ActualHeight;

                            tickEnd.X = a_Length;
                            tickEnd.Y = tickStart.Y;
                        }
                        else
                        {
                            //Use these coords for normal position (on the left of the chart)
                            tickStart.Y = a_TickToDraw.AxisPosition * this.ActualHeight;
                            tickStart.X = this.ActualWidth;

                            tickEnd.X = this.ActualWidth - a_Length;
                            tickEnd.Y = tickStart.Y;
                        }
                        gl.GuidelinesY.Add(tickStart.Y + (a_Pen.Thickness / 2));
                        break;
                }
            }

            drawingContext.PushGuidelineSet(gl);
            drawingContext.DrawLine(a_Pen, tickStart, tickEnd);
            drawingContext.Pop();

            return tickEnd;
        }
    }
}