using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Gusdor.Charting;

namespace Gusdor.Charting
{
    public class StateAxisProvider: AxisProvider
    {
        #region Members
        Axis m_ParentAxis = null;
        bool m_Flipped = false;

        public bool Flipped
        {
            get { return m_Flipped; }
            set { m_Flipped = value; }
        }
        public StateAxisProvider(Axis a_ParentAxis)
        {
            m_ParentAxis = a_ParentAxis;
        }
        #endregion

        protected override string OnTooltipAtPoint(double a_Value, Range a_Range, System.Windows.Media.Transform a_Transform, AxisDrawingArgs args)
        {
            //Get the number of significant figures to display on each axis
            Point sigFigs = a_Transform.GetSignificantFigures();

            if (args.AxisOrientation == System.Windows.Controls.Orientation.Horizontal)
                return string.Format("{0}", a_Value.ToString("F"+((int)sigFigs.X)));
            else
                return string.Format("{0}", a_Value.ToString("F"+((int)sigFigs.Y)));
        }

        protected override void OnCalculateTicks(TickList a_Ticks, Range a_Range, AxisDrawingArgs a_Options)
        {
            if (a_Range.Start <= 0 && a_Range.End >= 0)
            {
                //Get the fractional position
                double pos = (0 - a_Range.Start) / a_Range.Size;

                //add 1 as a tick
                a_Ticks.MajorTicks.Add(new TickList.Tick(m_Flipped ? "On" : "Off", pos));
            }
            //Only add 2 ticks - off and on.
            if (a_Range.Start <= 1 && a_Range.End >= 1)
            {
                //Get the fractional position
                double pos = (1 - a_Range.Start) / a_Range.Size;

                //add 1 as a tick
                a_Ticks.MajorTicks.Add(new TickList.Tick(m_Flipped ? "Off" : "On", pos));
            }
        }

        protected override string OnRangeToString(Range a_Range)
        {
            return "";
        }
    }
}
