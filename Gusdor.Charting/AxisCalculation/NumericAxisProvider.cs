using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Gusdor.Charting
{
    public class NumericAxisProvider: AxisProvider
    {

        #region Properties
        /// <summary>
        /// Gets or sets the number of minor ticks
        /// </summary>
        #region MinorTickCount
        private int m_MinorTickCount = 3;
        public int MinorTickCount
        {
            get { return m_MinorTickCount; }
            set 
            { 
                m_MinorTickCount = value;
                RaiseNeedsRenderEvents();
            }
        }
        #endregion
        
        #endregion
        private double CalculateInterval(double a_RangeSize)
        {
            //Use the range to determine the number of ticks to use.
            if (a_RangeSize < 10000000 && a_RangeSize > 1000000)
            {
                return 1000000;
            }
            if (a_RangeSize < 1000000 && a_RangeSize > 100000)
            {
                return 100000;
            }
            if (a_RangeSize < 100000 && a_RangeSize > 10000)
            {
                return 10000;
            }
            if (a_RangeSize < 10000 && a_RangeSize > 1000)
            {
                return 1000;
            }
            if (a_RangeSize < 1000 && a_RangeSize > 100)
            {
                return 100;
            }
            if (a_RangeSize < 100 && a_RangeSize > 10)
            {
                return 10;
            }
            if (a_RangeSize < 10 && a_RangeSize > 1)
            {
                return 1;
            }
            if (a_RangeSize < 1 && a_RangeSize > 0.1)
            {
                return 0.1;
            }
            if (a_RangeSize < 0.1 && a_RangeSize > 0.01)
            {
                return 0.01;
            }
            if (a_RangeSize < 0.01 && a_RangeSize > 0.001)
            {
                return 0.001;
            }
            if (a_RangeSize < 0.001 && a_RangeSize > 0.0001)
            {
                return 0.0001;
            }
            if (a_RangeSize < 0.0001 && a_RangeSize > 0.00001)
            {
                return 0.00001;
            }
            if (a_RangeSize < 0.00001 && a_RangeSize > 0.000001)
            {
                return 0.000001;
            }
            else
            {
                return a_RangeSize;
            }
        }

        int GetSigFigs(double a_RangeSize)
        {
            return Math.Max(1, (int)(Math.Ceiling(-Math.Log10(a_RangeSize)) + .1));
        }

        protected override string OnTooltipAtPoint(double a_Value, Range a_Range, Transform a_Transform, AxisDrawingArgs args)
        {
            //Get the number of significant figures to display on each axis
            Point sigFigs = a_Transform.GetSignificantFigures();

            if(args.AxisOrientation == System.Windows.Controls.Orientation.Horizontal)
                return string.Format(this.LabelFormatString, a_Value.ToString((int)sigFigs.X));
            else
                return string.Format(this.LabelFormatString, a_Value.ToString((int)sigFigs.Y));
        }

        protected override void OnCalculateTicks(TickList a_Tick, Range a_Range, AxisDrawingArgs a_Options)
        {
            double smallestInterval = 0.00001;

            //double valueInterval = CalculateInterval(a_Range.Size);
            double valueInterval = Math.Pow(10, Math.Floor(Math.Log10(a_Range.Size/10))) * 5.0;
            double positionInterval = (valueInterval / a_Range.Size);

            if (valueInterval < smallestInterval)
                return;

            //Snap to the smallest interval allowed if under it.
            //if (positionInterval < smallestInterval)
            //{
            //    double percentage = positionInterval / smallestInterval;

            //    valueInterval = valueInterval / percentage;
            //    positionInterval = smallestInterval;
            //}

            //Round start down starting tick to nearest interval.
            double tickStart = a_Range.Start - (a_Range.Start % valueInterval) - valueInterval;

            //Create ticks until beyond the range
            double tickValue = tickStart;
            while (tickValue < a_Range.End)
            {
                double tickPosition = (tickValue - a_Range.Start) / a_Range.Size;

                //Round tickvalue to 10 decimal places
                //Fix for Issue # PW-573
                tickValue = Math.Round(tickValue, 10);

                a_Tick.MajorTicks.Add(new TickList.Tick(tickValue, tickPosition));
                
                tickValue += valueInterval;
            }

            //MINOR TICKS
            if (a_Options.MinorTickCount > 0)
            {
                //Calculate the tick interval
                double minorTickPositionInterval = positionInterval / (a_Options.MinorTickCount + 1);
                double minorTickValueInterval = valueInterval / (a_Options.MinorTickCount + 1);

                //Create minor ticks
                foreach (TickList.Tick currentMajorTick in a_Tick.MajorTicks)
                {
                    double tickPosition = currentMajorTick.AxisPosition + minorTickPositionInterval;
                    tickValue = (double)currentMajorTick.Value + minorTickValueInterval;

                    //Only add the required number of ticks
                    for (int i = 1; i <= a_Options.MinorTickCount; i++)
                    {
                        a_Tick.MinorTicks.Add(new TickList.Tick(tickValue.ToString(), tickPosition));

                        tickPosition += minorTickPositionInterval;
                        tickValue += minorTickValueInterval;
                    }
                }
            }
        }

        protected override string OnRangeToString(Range a_Range)
        {
            return a_Range.Start + " - " + a_Range.End;
        }
    }
}
