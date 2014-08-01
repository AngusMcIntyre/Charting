using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gusdor.Charting
{
    public class Shader:ChartElementControl
    {

        public Pen BorderPen
        {
            get { return (Pen)GetValue(BorderPenProperty); }
            set { SetValue(BorderPenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderPen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderPenProperty =
            DependencyProperty.Register("BorderPen", typeof(Pen), typeof(Shader), new PropertyMetadata(new Pen()));
        
        /// <summary>
        /// Gets or sets the lower bottom of the rectangle area to be drawn. Set only this property to shade the left side of the chart
        /// </summary>
        [System.ComponentModel.Category("Common")]
        public Double? XLeft
        {
            get { return (Double?)GetValue(XLeftProperty); }
            set { SetValue(XLeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XLeftProperty =
            DependencyProperty.Register("XLeft", typeof(Double?), typeof(Shader), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        [System.ComponentModel.Category("Common")]
        public Double? XRight
        {
            get { return (Double?)GetValue(XRightProperty); }
            set { SetValue(XRightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XRightProperty =
            DependencyProperty.Register("XRight", typeof(Double?), typeof(Shader), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        [System.ComponentModel.Category("Common")]
        public Double? YBottom
        {
            get { return (Double?)GetValue(YBottomProperty); }
            set { SetValue(YBottomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YBottomProperty =
            DependencyProperty.Register("YBottom", typeof(Double?), typeof(Shader), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
       
        /// <summary>
        ///Gets or sets the top right point of the rectangle area to be drawn. Set only this property to shade the right side of the chart
        /// </summary>
        [System.ComponentModel.Category("Common")]
        public Double? YTop
        {
            get { return (Double?)GetValue(YTopProperty); }
            set { SetValue(YTopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YTopProperty =
            DependencyProperty.Register("YTop", typeof(Double?), typeof(Shader), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            Pen mypen = new Pen(new SolidColorBrush(Colors.Black),1);
            //base.OnRender(dc);
            if (ParentChart == null) return;

            var args = ParentChart.GetDrawingArgs();
            var renderbounds = args.RenderBounds;
            if (XLeft != null || XRight != null || YBottom != null || YTop != null)//at least one value has to be set
            {
                //assign a rect, regardless of actual null state of values so that transform only needs to be done once.
                Rect transformedPoint = args.Transform.TransformBounds(new Rect(
                    new Point(XLeft.HasValue? XLeft.Value: 0, YTop.HasValue ? YTop.Value : 0), 
                    new Point(XRight.HasValue? XRight.Value : 0, YBottom.HasValue ? YBottom.Value : 0)));

                Point topLeft = args.RenderBounds.TopLeft;
                Point bottomRight = args.RenderBounds.BottomRight;

                //check each value to see if it should be used in the final clip rectangle
                if (XLeft.HasValue)
                    topLeft = new Point(transformedPoint.Left, topLeft.Y);

                if (XRight.HasValue)
                    bottomRight = new Point(transformedPoint.Right, bottomRight.Y);

                if (YTop.HasValue)
                    topLeft = new Point(topLeft.X, transformedPoint.Top);

                if (YBottom.HasValue)
                    topLeft = new Point(bottomRight.X, transformedPoint.Bottom);

                var clipRegion = new Rect(topLeft, bottomRight);

                if (!clipRegion.Contains(args.RenderBounds))
                {
                    //todraw = args.Transform.TransformBounds(todraw);
                    CombinedGeometry cmg = new CombinedGeometry(new RectangleGeometry(renderbounds), new RectangleGeometry(clipRegion));
                    cmg.GeometryCombineMode = GeometryCombineMode.Exclude;

                    dc.DrawGeometry(this.Background, null, cmg);
                    dc.DrawRectangle(null, BorderPen, clipRegion);
                }
            }
  
        }

        protected override void OnChartAttached()
        {
            ParentChart.TransformChanged +=ParentChart_TransformChanged;
        }

        private void ParentChart_TransformChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }
        protected override void OnChartDettached(Chart oldChart)
        {
            if(oldChart != null)
                oldChart.TransformChanged -= ParentChart_TransformChanged;
        }

        public Shader()
        {
 
        }

    }
}
