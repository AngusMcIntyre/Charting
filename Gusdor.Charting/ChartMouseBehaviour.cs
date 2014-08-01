using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace Gusdor.Charting
{
    public class ChartMouseBehaviour: System.Windows.Interactivity.Behavior<Chart>
    {
        Point? m_PanPos;

        /// <summary>
        /// Gets or sets the cursor to use when panning.
        /// </summary>
        public System.Windows.Input.Cursor Cursor
        {
            get { return (System.Windows.Input.Cursor)GetValue(CursorProperty); }
            set { SetValue(CursorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.Register("Cursor", typeof(System.Windows.Input.Cursor), typeof(ChartMouseBehaviour), new PropertyMetadata(System.Windows.Input.Cursors.Hand));

        /// <summary>
        /// Gets or sets if the user can pan with the mouse.
        /// </summary>
        public bool UseMousePan
        {
            get { return (bool)GetValue(UseMousePanProperty); }
            set { SetValue(UseMousePanProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseMousePan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseMousePanProperty =
            DependencyProperty.Register("UseMousePan", typeof(bool), typeof(ChartMouseBehaviour), new PropertyMetadata(true));

        //Gets or sets the amount to zoom in or out by when the mouse wheel is scrolled.
        public double ZoomAmount
        {
            get { return (double)GetValue(ZoomAmountProperty); }
            set { SetValue(ZoomAmountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomAmount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomAmountProperty =
            DependencyProperty.Register("ZoomAmount", typeof(double), typeof(ChartMouseBehaviour), new PropertyMetadata(0.1));

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.MouseDown += AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseUp += AssociatedObject_MouseLeftButtonUp;
            this.AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            this.AssociatedObject.MouseWheel += AssociatedObject_MouseWheel;
        }

        void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var zoom = new Size(AssociatedObject.ViewArea.Width * ZoomAmount, AssociatedObject.ViewArea.Height * ZoomAmount);

            if (e.Delta < 0)
                AssociatedObject.ViewArea = Rect.Inflate(AssociatedObject.ViewArea, zoom);
            else if (e.Delta > 0)
                AssociatedObject.ViewArea = Rect.Inflate(AssociatedObject.ViewArea, -zoom.Width, -zoom.Height);
        }

        void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (m_PanPos != null)
            {
                var pos = e.GetPosition(this.AssociatedObject);

                var vec = pos - m_PanPos;

                if(vec.HasValue)
                {
                    var grp = new TransformGroup();
                    grp.Children.Add(this.AssociatedObject.Transform);
                    grp.Children.Add(new TranslateTransform(vec.Value.X, vec.Value.Y));

                    this.AssociatedObject.Transform = new MatrixTransform(grp.Value);

                    m_PanPos = pos;
                }
            }
        }

        void AssociatedObject_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                System.Windows.Input.Mouse.Capture(null);
                m_PanPos = null;
                this.AssociatedObject.Cursor = null;
            }
        }

        void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (UseMousePan)
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                {
                    this.AssociatedObject.Cursor = this.Cursor;
                    m_PanPos = e.GetPosition(this.AssociatedObject);
                    Mouse.Capture(this.AssociatedObject);
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.MouseDown -= AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseUp-= AssociatedObject_MouseLeftButtonUp;
            this.AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            this.AssociatedObject.MouseWheel -= AssociatedObject_MouseWheel;
        }
    }
}
