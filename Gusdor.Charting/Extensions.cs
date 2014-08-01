using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Gusdor.Charting
{
    public static class Extensions
    {
        /// <summary>
        /// Rounds a DateTime to the given unit.
        /// </summary>
        /// <param name="d">DateTime instance to round</param>
        /// <param name="a_RoundingType">Unit to round to</param>
        /// <returns>Rounded DateTime</returns>
        public static DateTime Round(this DateTime d, TimeUnit a_RoundingType, bool a_AllowRoundUp)
        {
            DateTime dtRounded = new DateTime();

            switch (a_RoundingType)
            {
                case TimeUnit.Seconds:
                    {
                        dtRounded = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
                        if (d.Millisecond >= 500 && a_AllowRoundUp)
                            dtRounded = dtRounded.AddSeconds(1);

                        break;
                    }
                case TimeUnit.Minutes:
                    {
                        dtRounded = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);
                        if (d.Second >= 30 && a_AllowRoundUp)
                            dtRounded = dtRounded.AddMinutes(1);

                        break;
                    }
                case TimeUnit.QuarterHour:
                    {
                        dtRounded = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);

                        double minuteRemainder = dtRounded.Minute % 15;

                        if (minuteRemainder != 0)
                            dtRounded = dtRounded.AddMinutes(-minuteRemainder);

                        if ( (minuteRemainder >= 7) && (d.Second >= 30) && a_AllowRoundUp)
                            dtRounded = dtRounded.AddMinutes(15);
                        break;
                    }
                case TimeUnit.Hours:
                    {
                        dtRounded = new DateTime(d.Year, d.Month, d.Day, d.Hour, 0, 0);
                        if (d.Minute >= 30 && a_AllowRoundUp)
                            dtRounded = dtRounded.AddHours(1);

                        break;
                    }
                case TimeUnit.ThreeHours:
                    {
                        dtRounded = new DateTime(d.Year, d.Month, d.Day, d.Hour, 0, 0);

                        double hourRemainder = dtRounded.Hour % 3;
                        if (hourRemainder != 0)
                            dtRounded.AddHours(-hourRemainder);  

                        if (hourRemainder >= 1.5 && a_AllowRoundUp)
                            dtRounded = dtRounded.AddHours(3);
                        break;
                    }
                case TimeUnit.Days:
                    {
                        dtRounded = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
                        if (d.Hour >= 12 && a_AllowRoundUp)
                            dtRounded = dtRounded.AddDays(1);

                        break;
                    }
                case TimeUnit.Weeks:
                    {
                        //Round to day
                        dtRounded = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);

                        //Quciker than a switch. Cast DayOfWeek to int (Monday = 1)
                        dtRounded = dtRounded.AddDays(-((int)dtRounded.DayOfWeek - 1));

                        if ((int)d.DayOfWeek >= 4 && a_AllowRoundUp)
                            dtRounded = dtRounded.AddDays(7);

                        break;
                    }
                case TimeUnit.Months:
                    {
                        dtRounded = new DateTime(d.Year, d.Month, 1, 0, 0, 0);
                        if (d.Day >= 15 && a_AllowRoundUp)
                            dtRounded = dtRounded.AddMonths(1);

                        break;
                    }
                case TimeUnit.Years:
                    {
                        dtRounded = new DateTime(d.Year, 1, 1, 0, 0, 0);
                        if (d.Month >= 6 && a_AllowRoundUp)
                            dtRounded = dtRounded.AddDays(1);

                        break;
                    }
            }

            return dtRounded;
        }

        /// <summary>
        /// Returns a string representation to the given number of significant figures.
        /// </summary>
        /// <param name="a_SigFigs">The number of significant figures to format</param>
        /// <returns>Format string</returns>
        public static string ToString(this double a_Value, int a_SigFigs)
        {
            string format = "F" + a_SigFigs.ToString();
            return a_Value.ToString(format);
        }

        /// <summary>
        /// Calculates the number of significant figures necessary to display 1 device independant pixel with accuracy.
        /// </summary>
        /// <returns>Number of significant figures</returns>
        public static Point GetSignificantFigures(this System.Windows.Media.Transform a_Transform)
        {
            if (a_Transform.Inverse != null)
            {
                Rect rect = a_Transform.Inverse.TransformBounds(new Rect(0, 0, 1, 1));

                return new Point(Math.Max(1, (int)(Math.Ceiling(-Math.Log10(rect.Width)) + .1)), Math.Max(1, (int)(Math.Ceiling(-Math.Log10(rect.Height)) + .1)));
            }
            return new Point();
        }

        /// <summary>
        /// Calculate the chart space dimensions of a single pixel.
        /// </summary>
        /// <param name="a_Transform"></param>
        /// <returns></returns>
        public static Vector GetPixelDimensions(this System.Windows.Media.Transform a_Transform)
        {
            Rect dim = a_Transform.Inverse.TransformBounds(new Rect(0, 0, 1, 1));
            return new Vector(dim.X, dim.Y);
        }

        /// <summary>
        /// Tests to see if the line between two points intersets the rectangle
        /// </summary>
        /// <param name="newRegion"></param>
        /// <param name="a_FirstPoint"></param>
        /// <param name="a_SecondPoint"></param>
        /// <returns></returns>
        public static bool DoesLineIntersect(this Rect region, Point a_FirstPoint, Point a_SecondPoint)//, out Point[] a_InterSectPoint)
        {
            bool edgeAccepted = false;

            //Check each edge for an intersection
            //Top edge
            if (!edgeAccepted)
            {
                double edgeProportionToIntersect = (a_SecondPoint.Y - region.Bottom) / (a_SecondPoint.Y - a_FirstPoint.Y);
                double xcoord = 0.0;
                //Check that an intersect would occur if the line is indefinite
                if (edgeProportionToIntersect >= 0.0 && edgeProportionToIntersect <= 1.0)
                {
                    xcoord = a_FirstPoint.X + (edgeProportionToIntersect * (a_FirstPoint.X - a_SecondPoint.X));

                    //Check that the y coordinate falls on the line
                    if (xcoord >= region.Left && xcoord <= region.Right)
                        edgeAccepted = true;
                }
            }
            //Bottom edge
            if (!edgeAccepted)
            {
                double edgeProportionToIntersect = (a_SecondPoint.Y - region.Top) / (a_SecondPoint.Y - a_FirstPoint.Y);
                double xcoord = 0.0;
                //Check that an intersect would occur if the line is indefinite
                if (edgeProportionToIntersect >= 0.0 && edgeProportionToIntersect <= 1.0)
                {
                    xcoord = a_FirstPoint.X + (edgeProportionToIntersect * (a_FirstPoint.X - a_SecondPoint.X));

                    //Check that the y coordinate falls on the line
                    if (xcoord >= region.Left && xcoord <= region.Right)
                        edgeAccepted = true;
                }
            }
            //Left edge
            if (!edgeAccepted)
            {
                double edgeProportionToIntersect = (a_SecondPoint.X - region.Left) / (a_SecondPoint.X - a_FirstPoint.X);
                double ycoord = 0.0;
                //Check that an intersect would occur if the line is indefinite
                if (edgeProportionToIntersect >= 0.0 && edgeProportionToIntersect <= 1.0)
                {
                    ycoord = a_FirstPoint.Y + (edgeProportionToIntersect * (a_FirstPoint.Y - a_SecondPoint.Y));

                    //Check that the y coordinate falls on the line
                    if (ycoord >= region.Top && ycoord <= region.Bottom)
                        edgeAccepted = true;
                }
            }
            //Right edge
            if (!edgeAccepted)
            {
                double edgeProportionToIntersect = (a_SecondPoint.X - region.Right) / (a_SecondPoint.X - a_FirstPoint.X);
                double ycoord = 0.0;
                //Check that an intersect would occur if the line is indefinite
                if (edgeProportionToIntersect >= 0.0 && edgeProportionToIntersect <= 1.0)
                {
                    ycoord = a_FirstPoint.Y + (edgeProportionToIntersect * (a_FirstPoint.Y - a_SecondPoint.Y));

                    //Check that the y coordinate falls on the line
                    if (ycoord >= region.Top && ycoord <= region.Bottom)
                        edgeAccepted = true;
                }
            }
            return edgeAccepted;
        }

        #region List<TickList.Tick>
        /// <summary>
        /// Returns all TickList.Ticks with Axis Positions equal or between 0 and 1.
        /// </summary>
        public static List<TickList.Tick> GetOnScreenTicks(this List<TickList.Tick> a_Ticks)
        {
            List<TickList.Tick> returnTicks = new List<TickList.Tick>();

            foreach (TickList.Tick currentTick in a_Ticks)
            {
                if (currentTick.AxisPosition >= 0 && currentTick.AxisPosition <= 1)
                    returnTicks.Add(currentTick);
            }

            return returnTicks;
        }
        #endregion

        #region FormattedText
        /// <summary>
        /// Gets the actual width of the FormattedText when it is draw to screen.
        /// </summary>
        /// <param name="a_Text"></param>
        /// <returns></returns>
        public static double GetActualStringWidth(this System.Windows.Media.FormattedText a_Text)
        {
            return a_Text.Width - a_Text.OverhangLeading - a_Text.OverhangTrailing;
        }

        /// <summary>
        /// Gets the offset necessary to centre the text based upon its orientation.
        /// </summary>
        /// <param name="a_Orientation">The orientation of the axis the tick is associated with.</param>
        /// <returns></returns>
        public static double GetOffsetToCentre(this System.Windows.Media.FormattedText a_Text, AxisDrawingArgs args)
        {
            if (args.AxisOrientation == System.Windows.Controls.Orientation.Horizontal)
                return -(a_Text.OverhangLeading + (a_Text.GetActualStringWidth() / 2));
            else
                return -(a_Text.Height / 2);
        }

        /// <summary>
        /// Gets the number of lines represented by the FormattedText, given the MaxTextWidth property.
        /// </summary>
        public static int GetNumberOfLines(this System.Windows.Media.FormattedText a_Text)
        {
            if (a_Text.LineHeight == 0.0)
                return 1;

            return (int)Math.Ceiling(a_Text.Height / a_Text.LineHeight);
        }

        /// <summary>
        /// Returns True if the MaxTextWidth is too small to display the text without ellipses characters.
        /// </summary>
        public static bool IsTextOccluded(this System.Windows.Media.FormattedText a_Text)
        {
            return a_Text.MinWidth >= a_Text.MaxTextWidth;
        }
        #endregion
    }
}
