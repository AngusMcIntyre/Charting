using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace Gusdor.Charting
{
    public class ChartCursorKeyPanBehaviour : System.Windows.Interactivity.Behavior<Chart>
    {
        public double Speed
        {
            get { return (double)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Speed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(double), typeof(ChartCursorKeyPanBehaviour), new PropertyMetadata(1.0));

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            Vector? sp = null;
            
            switch (e.Key)
            {
                case Key.Left:
                    sp = new Vector(-Speed, 0.0);
                    break;
                case Key.Right:
                    sp = new Vector(Speed, 0.0);
                    break;
                case Key.Up:
                    sp = new Vector(0.0, Speed);
                    break;
                case Key.Down:
                    sp = new Vector(0.0, -Speed);
                    break;
            }

            if(sp != null)
            {
                var grp = new TransformGroup();
                grp.Children.Add(this.AssociatedObject.Transform);
                grp.Children.Add(new TranslateTransform(sp.Value.X, sp.Value.Y));

                this.AssociatedObject.Transform = new MatrixTransform(grp.Value);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }
    }
}
