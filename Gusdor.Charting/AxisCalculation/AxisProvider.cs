using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using System.ComponentModel;

namespace Gusdor.Charting
{
    /// <summary>
    /// Abstract declaration of the AxisProvider class, enabling host application to create custom tick calculation.
    /// </summary>
    public abstract class AxisProvider: INotifyPropertyChanged
    {
        #region Members
        protected string m_Label = null;
        protected string m_UnitLabel = null;
        protected string m_UnitAbbreviation = null;    
        #endregion

        public TickList Ticks
        {
            get { return m_Ticks; }
            set
            {
                m_Ticks = value;

                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Ticks"));
            }
        } TickList m_Ticks = default(TickList);

        #region Properties
        public string LabelFormatString { get; set; }

        #region Offset
        private double m_Offset = 0.0;
        /// <summary>
        /// Gets or sets an offset value for the range displayed on this axes
        /// </summary>
        public double Offset
        {
            get { return m_Offset; }
            set 
            { 
                m_Offset = value;
                if(this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Offset"));
            }
        }
        #endregion
     
        #region LineScale
        private double m_Scale = 1.0;
        /// <summary>
        /// Gets or sets a value to set the scale of the range represented on the axis.
        /// </summary>
        public double Scale
        {
            get { return m_Scale; }
            set 
            { 
                m_Scale = value;
                if(this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Scale"));
            }
        }
        #endregion        
  
        #endregion

        #region Events
        public delegate void AxisNeedsRenderDelegate();
        public event AxisNeedsRenderDelegate AxisNeedsRender;
        #endregion     

        public AxisProvider()
        {
            LabelFormatString = "{0}";
        }

        internal FormattedText CreateLabel(string a_Text, AxisDrawingArgs args)
        {
            if (a_Text == null)
                a_Text = "";

            FormattedText text = new FormattedText(a_Text, CultureInfo.CurrentUICulture,
                                      FlowDirection.LeftToRight,
                                      new Typeface(args.LabelFontFamily.ToString()),
                                      args.LabelFontSize, args.LabelColour);

            text.Trimming = TextTrimming.CharacterEllipsis;
            return text;
        }

        protected void RaiseNeedsRenderEvents()
        {
            if(this.AxisNeedsRender != null)
                this.AxisNeedsRender();
        }

        protected static double FractionToPoint(double p, double axisLength)
        {
            return axisLength * p;
        }
        /// <summary>
        /// Calculates an appropriate tooltip for the given value to be displayed at the cursor.
        /// </summary>
        /// <param name="a_Value">Value of the desired label</param>
        /// <param name="a_Range">Currently displayed range</param>
        /// <param name="a_Transform">Host charts transform. Included in order to support significant figure calculations.</param>
        /// <param name="a_Orientation">Orientation of the axis</param>
        /// <returns>String label.</returns>
        public string TooltipAtPoint(double a_Value, Range a_Range, Transform a_Transform, AxisDrawingArgs args)
        {
            return OnTooltipAtPoint(a_Value, TransformRange(a_Range), a_Transform, args);
        }
        /// <summary>
        /// Calculates an appropriate tooltip for the given value to be displayed at the cursor.
        /// </summary>
        /// <param name="a_Value">Value of the desired label</param>
        /// <param name="a_Range">Currently displayed range</param>
        /// <param name="a_Transform">Host charts transform. Included in order to support significant figure calculations.</param>
        /// <param name="a_Orientation">Orientation of the axis</param>
        /// <returns>String label.</returns>
        protected abstract string OnTooltipAtPoint(double a_Value, Range a_Range, Transform a_Transform, AxisDrawingArgs args);
        /// <summary>
        /// Returns a string representation of a Range's Size, in the format of the AxisProvider.
        /// </summary>
        public string RangeToString(Range a_Range)
        {
            return OnRangeToString(TransformRange(a_Range));
        }
        /// <summary>
        /// Returns a string representation of a Range's Size, in the format of the AxisProvider.
        /// </summary>
        protected abstract string OnRangeToString(Range a_Range);

        public void CalculateTicks(Range a_Range, AxisDrawingArgs args)
        {
            TickList ticks = new TickList();

            //Prevent divide-by-zero exceptions.
            if (a_Range.Size > 0)
            {
                //Transform the range
                OnCalculateTicks(ticks, TransformRange(a_Range), args);
            }

            Ticks = ticks;
        }

        protected abstract void OnCalculateTicks(TickList a_Ticks, Range a_Range, AxisDrawingArgs a_Options);

        protected Range TransformRange(Range a_RangeToTransform)
        {
            return new Range(
                SingleAxisTransform.Transform(a_RangeToTransform.Start, this.Offset, this.Scale),
                SingleAxisTransform.Transform(a_RangeToTransform.End, this.Offset, this.Scale));
        }
    
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler  PropertyChanged;

        #endregion

        #region Events
        void Transform_TransformChanged(object sender, System.EventArgs e)
        {
            RaiseNeedsRenderEvents();
        } 
        #endregion
}

    public class TickList
    {
        List<Tick> m_MajorTicks = new List<Tick>();
        List<Tick> m_MinorTicks = new List<Tick>();

        public List<Tick> MinorTicks
        {
            get { return m_MinorTicks; }
        }
        public List<Tick> MajorTicks
        {
            get { return m_MajorTicks; }
        }

        public override string ToString()
        {
            return string.Format("{0} Major Ticks, {1} Minor Ticks", m_MajorTicks.Count, m_MinorTicks.Count);
        }
    
        public class Tick : IComparable
        {
            public Tick(object value, double position)
            {
                Value = value;
                AxisPosition = position;
            }

            public object Value
            {
                get;
                set;
            }

            public double AxisPosition
            {
                get;
                set;
            }

            public override string ToString()
            {
                return string.Format("Value: {0}, Position: {1}", Value.ToString(), AxisPosition);
            }

            #region IComparable Members

            public int CompareTo(object obj)
            {
                if (this.AxisPosition < (obj as Tick).AxisPosition) 
                    return -1;
                if (this.AxisPosition > (obj as Tick).AxisPosition)
                    return 1;
                else
                    return 0;
            }

            #endregion
        }
    }

    public class Range: INotifyPropertyChanged
    {
        double m_Start = 0.0;
        double m_End = 0.0;

        public Range(double start, double end)
        {
            m_Start = start;
            m_End = end;

            this.NotitifyPropertiesChanged();
        }
        public Range() { }

        public double Start
        {
            get
            {
                return m_Start;
            }
            set
            {
                m_Start = value;

                this.NotitifyPropertiesChanged();
            }
        }

        public double End
        {
            get
            {
                return m_End;
            }
            set
            {
                m_End = value;
                this.NotitifyPropertiesChanged();
            }
        }

        public double Size
        {
            get 
            {
                return Math.Abs(m_End - m_Start);
            }
        }

        /// <summary>
        /// Tests a value, if it falls outside the bounds of the range, the bonuds are updated with the new value.
        /// </summary>
        public void AddBound(double a_Value)
        {
            if (a_Value > this.m_End)
                m_End = a_Value;

            if (a_Value < this.m_Start)
                m_Start = a_Value;
        }

        public override string ToString()
        {
            return string.Format("Start {0}, End {1}, Size {2}", this.Start, this.End, this.Size);
        }

        private void NotitifyPropertiesChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Start"));
                PropertyChanged(this, new PropertyChangedEventArgs("Centre"));
                PropertyChanged(this, new PropertyChangedEventArgs("End"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
