using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Gusdor.Charting
{
    public class DrawingArgs: EventArgs
    {
        public Transform Transform { get; private set; }
        /// <summary>
        /// Gets the viewable, chart space value range.
        /// </summary>
        public System.Windows.Rect ViewRange { get; set; }
        /// <summary>
        /// Gets the render space bounds of the chart.
        /// </summary>
        public System.Windows.Rect RenderBounds { get; set; }

        public DrawingArgs(Transform transform, System.Windows.Rect viewRange, System.Windows.Rect renderBounds)
        {
            Transform = transform;
            ViewRange = viewRange;
            RenderBounds = renderBounds;
        }
    }
}
