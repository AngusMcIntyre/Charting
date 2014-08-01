using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gusdor.Charting
{
    public class AxisDrawingArgs: EventArgs
    {
        public System.Windows.Controls.Orientation AxisOrientation { get; set; }

        public System.Windows.Media.Brush LabelColour { get; set; }

        public double LabelFontSize { get; set; }

        public Range Range { get; set; }

        public int MinorTickCount { get; set; }

        public System.Windows.Media.FontFamily LabelFontFamily { get; set; }

        public System.Windows.FontWeight LabelFontWeight { get; set; }

        public System.Windows.Media.FontFamily LabelFontStyle { get; set; }
    }
}
