using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gusdor.Charting;
using System.Windows.Media;
using System.Globalization;
using System.ComponentModel;

namespace Gusdor.Charting
{
    /// <summary>
    /// Axiscalculator for the calculation of datetime axis readouts. To be used with Gusdor.Charting.Axis.
    /// It is important to remember that the assume unit value is a day. Therefore 1 == 1 day.
    /// </summary>
    public class DateTimeAxisCalculator: AxisProvider, INotifyPropertyChanged
    {
        #region Members
        DateTime m_ZeroValue = DateTime.Now.Date;
        public DateTime ZeroValueDate
        {
            get { return m_ZeroValue; }
            set { m_ZeroValue = value; this.RaiseNeedsRenderEvents(); }
        }
        #endregion

        #region Properties

        public DateTime ZeroDate { get; set; }     
        
        #region DisplayType
        private DateDisplay m_DisplayType = DateDisplay.Auto;
        public DateDisplay DisplayType
        {
            get { return m_DisplayType; }
            set 
            { 
                m_DisplayType = value;

                if(this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("DisplayType"));

                RaiseNeedsRenderEvents();
            }
        }
        #endregion
       
        #region LabelFormat
        private string m_LabelFormat = "dd/MM/yy HH:mm";
        public string LabelFormat
        {
            get { return m_LabelFormat; }
            set 
            { 
                m_LabelFormat = value;
                if(this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("LabelFormat"));
            }
        }
        #endregion    

        public string YearsLabelFormat
        {
            get { return m_YearsLabelFormat; }
            set
            {
                m_YearsLabelFormat = value;
            }
        } string m_YearsLabelFormat = "yyyy";

        public string MonthAndYearLabelFormat
        {
            get { return m_MonthAndYearLabelFormat; }
            set
            {
                m_MonthAndYearLabelFormat = value;
            }
        } string m_MonthAndYearLabelFormat = "MMM yyyy";

        public string ShortDateLabelFormat
        {
            get { return m_ShortDateLabelFormat; }
            set
            {
                m_ShortDateLabelFormat = value;
            }
        } string m_ShortDateLabelFormat = "ddd" + " " + CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

        public string ShortDateAndTimeLabelFormat
        {
            get { return m_ShortDateAndTimeLabelFormat; }
            set
            {
                m_ShortDateAndTimeLabelFormat = value;
            }
        } string m_ShortDateAndTimeLabelFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + Environment.NewLine + CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

        public string LongTimeLabelFormat
        {
            get { return m_LongTimeLabelFormat; }
            set
            {
                m_LongTimeLabelFormat = value;
            }
        } string m_LongTimeLabelFormat =
            CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + Environment.NewLine + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;

        public string SubSecondLabelFormat
        {
            get { return m_SubSecondLabelFormat; }
            set
            {
                m_SubSecondLabelFormat = value;
            }
        } string m_SubSecondLabelFormat =
            CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + Environment.NewLine + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern + ".ffff";
        #endregion

        #region Constructor
        /// <summary>
        /// Uses 
        /// </summary>
        public DateTimeAxisCalculator() : this(DateTime.Now.Date) { }
        /// <param name="a_ZeroDate">The base value of 0.0 on the axis.</param>
        public DateTimeAxisCalculator(DateTime a_ZeroDate)
            : base()
        {
            m_ZeroValue = a_ZeroDate;
        } 
        #endregion

        #region Overrides
        //internal override void CalculateTicks(TickList a_Ticks, Range a_Range, AxisDrawOptions args)
        //{
        //    //Find the start value (left or bottom ) of the axis by 
        //    //adding the range start to the zerovalue.
        //    DateTime rangeStartDate = m_ZeroValue.AddDays(a_Range.Start);

        //    //Get the rounding interval in order to normalise the start value
        //    double intervalSize = 0.0;

        //    TimeUnit tickInterval = this.GetInterval(a_Range.Size, out intervalSize);
        //    //Round the datetime
        //    DateTime roundedStartDate = rangeStartDate.Round(tickInterval, false);

        //    //Adjust range start value so that it reflects the rounded value
        //    double roundedStartValue = roundedStartDate.Subtract(m_ZeroValue).TotalDays;
        //    double cumulativeValue = roundedStartValue;

        //    //Build the ticks
        //    while (true)
        //    {
        //        //Determine the tick's position
        //        double tickPos = (cumulativeValue - a_Range.Start) / a_Range.Size;

        //        //Break loop if current tick is placed > 1
        //        if (tickPos > 1)
        //            break;

        //        //Assemble the tick's label value
        //        DateTime tickDate = m_ZeroValue.AddDays(cumulativeValue);
        //        string tickValue = string.Empty;

        //        switch (m_DisplayType)
        //        {
        //            case DateDisplay.TimeOnly:
        //                tickValue = tickDate.ToShortTimeString();
        //                break;
        //            case DateDisplay.DateOnly:
        //                tickValue = tickDate.ToShortDateString();
        //                break;
        //            case DateDisplay.Both:
        //                tickValue = tickDate.ToShortDateString() + " " + tickDate.ToShortTimeString();
        //                break;
        //        }

        //        //Add the tick
        //        a_Ticks.MajorTicks.Add(new TickList.Tick(tickValue, tickPos));

        //        //Increment the cumulative value by the interval amount
        //        cumulativeValue += intervalSize;
        //    }
        //}

        protected override void OnCalculateTicks(TickList a_Ticks, Range a_Range, AxisDrawingArgs args)
        {
            try
            {
                int minorTickCount = args.MinorTickCount;

                //Find the start value (left or bottom ) of the axis by 
                //adding the range start to the zerovalue.
                string autoFormat = "";
                //Get the rounding interval in order to normalise the start value
                TimeSpan intervalDuration = GetInterval(a_Range.Size, out autoFormat);
                double intervalSize = Math.Abs(intervalDuration.TotalDays);

                //Break if zoom is too small
                if (intervalSize == 0)
                    return;

                //Round down to snap to interval using datetime ticks (using raw double value did not work correctly due to rounding)
                DateTime rangeStartTime = this.DoubleToDateTime(a_Range.Start);
                DateTime roundedStartTime = new DateTime(rangeStartTime.Ticks - (rangeStartTime.Ticks % intervalDuration.Ticks) - intervalDuration.Ticks);
                //double startValue = a_Range.Start - (Math.Abs(a_Range.Start) % intervalSize);
                double startValue = this.DateTimeToDouble(roundedStartTime);

                if (double.IsNaN(startValue))
                    startValue = 0.0;

                double cumulativeValue = startValue;

                double majorTickPosInterval = intervalSize / a_Range.Size;

                bool firstTick = true;

                //Build the ticks
                while (true)
                {
                    //Determine the tick's position
                    double tickPos = (cumulativeValue - a_Range.Start) / a_Range.Size;

                    //Break loop if current tick is placed >= 1
                    if (tickPos >= 1)
                        break;

                    //Assemble the tick's label value
                    DateTime tickDate = m_ZeroValue.AddDays(cumulativeValue);
                    string tickValue = GetString(tickDate, autoFormat);

                    //Add the tick
                    a_Ticks.MajorTicks.Add(new TickList.Tick(tickValue, tickPos));

                    //Increment the cumulative value by the interval amount
                    cumulativeValue += intervalSize;

                    if (args.MinorTickCount > 0)
                    {
                        double minorPosInterval = majorTickPosInterval / (minorTickCount + 1);

                        //Add minor ticks after this major tick
                        for (int i = 0; i < minorTickCount; i++)
                        {
                            a_Ticks.MinorTicks.Add(new TickList.Tick(null, tickPos + (minorPosInterval * (i + 1))));
                        }

                        if (firstTick)
                        {
                            //Add minor ticks after this major tick
                            for (int i = 0; i < minorTickCount; i++)
                            {
                                a_Ticks.MinorTicks.Add(new TickList.Tick(null, (tickPos - majorTickPosInterval) + (minorPosInterval * (i + 1))));
                            }
                        }
                    }

                    firstTick = false;
                }
            }
            catch// (DivideByZeroException ex)
            {
                //zoom too small.
            }
            //catch (ArgumentOutOfRangeException ex)
            //{
            //    //zoom too large.
            //}
        }

        protected override string OnTooltipAtPoint(double a_Value, Range a_Range, Transform a_Transform, AxisDrawingArgs args)
        {
            if (LabelFormat == null)
                return string.Empty;

            DateTime value = m_ZeroValue.AddDays(a_Value);

            return value.ToString(LabelFormat);
        }

        protected override string OnRangeToString(Range a_Range)
        {
            DateTime XStart = this.DoubleToDateTime(a_Range.Start);
            DateTime XEnd = this.DoubleToDateTime(a_Range.End);

            return string.Format("{0} {1} - {2} {3}",
                                        XStart.ToShortDateString(), XStart.ToShortTimeString(),
                                        XEnd.ToShortDateString(), XEnd.ToShortTimeString());
        }
        #endregion

        #region Private Methods
        private TimeSpan GetInterval(double a_Range, out string a_DateStringFormat)
        {
            //Define interval scales
            TimeSpan[] timeScales = 
            {
                new TimeSpan(365, 0, 0, 0),
                new TimeSpan(30, 0, 0, 0),
                new TimeSpan(7, 0, 0, 0),
                new TimeSpan(1, 0, 0, 0),
                new TimeSpan(12, 0, 0),
                new TimeSpan(6, 0, 0),
                new TimeSpan(3, 0, 0),
                new TimeSpan(1, 0, 0),
                new TimeSpan(0, 30, 0),
                new TimeSpan(0, 15, 0),
                new TimeSpan(0, 10, 0),
                new TimeSpan(0, 5, 0),
                new TimeSpan(0, 1, 0),
                new TimeSpan(0, 0, 30),
                new TimeSpan(0, 0, 1),
                new TimeSpan(0, 0, 0, 0, 1)
            };

            //Optimum number of major ticks at a time is 10
            double nominalInterval = a_Range / 10;

            int timesDivided = int.MaxValue;
            TimeSpan bestmatch = new TimeSpan();

            //Divide range by ten and examine which time scale divides in fully with the smallest remainder
            foreach (TimeSpan ts in timeScales)
            {
                if(nominalInterval > ts.TotalDays)
                {
                    //double rem = ts.TotalDays % nominalInterval;
                    int div = (int)Math.Abs(Math.Floor(nominalInterval / ts.TotalDays));

                    if(div >= 1 && ts > bestmatch)
                    {
                        timesDivided = div;
                        bestmatch = ts;
                    }
                }
            }

            TimeSpan intervalTimeSpan = new TimeSpan(bestmatch.Ticks * timesDivided);
            double interval = bestmatch.TotalDays * timesDivided;

            //Configure label format
            if (intervalTimeSpan.TotalDays >= 365)
            {
                a_DateStringFormat = YearsLabelFormat;              
            }
            else if (intervalTimeSpan.TotalDays >= 30)
            {
                a_DateStringFormat = MonthAndYearLabelFormat;
            }
            if (intervalTimeSpan.TotalDays >= 1)
            {
                a_DateStringFormat = ShortDateLabelFormat;
            }
            else if (intervalTimeSpan.TotalMinutes >= 1)
            {
                a_DateStringFormat = ShortDateAndTimeLabelFormat;
            }
            else if (intervalTimeSpan.TotalSeconds >= 1)
            {
                a_DateStringFormat = LongTimeLabelFormat;
            }
            else
                a_DateStringFormat = SubSecondLabelFormat;

            //return Math.Abs(interval);
            return intervalTimeSpan;
        }

        private string GetString(DateTime value, string autoFormatString)
        {
            switch (m_DisplayType)
            {
                case DateDisplay.Auto:
                    return value.ToString(autoFormatString);
                case DateDisplay.TimeOnly:
                    return value.ToShortTimeString();
                case DateDisplay.DateOnly:
                    return value.ToShortDateString();
                case DateDisplay.Both:
                    return value.ToShortDateString() + Environment.NewLine + value.ToShortTimeString();
            }

            return value.ToString();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Converts a DateTime to double, based upon the difference in days between the stored offset and Value parameter.
        /// </summary>
        /// <param name="a_Value">Value to be converted</param>
        /// <returns>Chart Value, in 'days from the offset'</returns>
        public double DateTimeToDouble(DateTime a_Value)
        {
            return a_Value.Subtract(m_ZeroValue).TotalDays;
        }

        /// <summary>
        /// Converts a double to a Dateime by returning the sum of the given value in days, and the constructor set ZeroValue.
        /// </summary>
        /// <param name="a_Value">Number of days relative to the ZeroValue of the requested DateTime</param>
        public DateTime DoubleToDateTime(double a_Value)
        {
            return m_ZeroValue.AddDays(a_Value);
        }
        #endregion

        #region INotifyPropertyChanged Members

        public new event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public enum DateDisplay
    {
        Auto,
        TimeOnly,
        DateOnly,
        Both
    }
}
