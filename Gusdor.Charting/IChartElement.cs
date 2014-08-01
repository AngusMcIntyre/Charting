using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Gusdor.Charting
{
    public interface IChartElement
    {
        event EventHandler Changed;

        void Draw(DrawingContext context, DrawingArgs args);
    }
}
