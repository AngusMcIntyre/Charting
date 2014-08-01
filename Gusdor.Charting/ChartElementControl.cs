using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Gusdor.Charting
{
    public abstract class ChartElementControl: System.Windows.Controls.Panel
    {
        public Chart ParentChart
        {
            get { return (Chart)GetValue(ParentChartProperty); }
            set { SetValue(ParentChartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentChartProperty =
            DependencyProperty.Register("ParentChart", typeof(Chart), typeof(ChartElementControl), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnParentChartChanged));

        static void OnParentChartChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as ChartElementControl;

            if(e.NewValue != null)
                item.OnChartAttached();

            if (e.OldValue != null)
                item.OnChartDettached((Chart)e.OldValue);
        }

        public ChartElementControl()
        {
            this.IsHitTestVisible = false;

            this.Loaded += ChartElementControl_Loaded;
        }

        void ChartElementControl_Loaded(object sender, RoutedEventArgs e)
        {
            var b = BindingOperations.GetBindingExpression(this, ParentChartProperty);
            if (b == null && ParentChart == null)
                //ParentChart = FindParentChart();
                SetParentChartBinding();
        }


        protected virtual void OnChartAttached()
        {
            this.ParentChart.TransformChanged += ParentChart_TransformChanged;
        }

        protected virtual void OnChartDettached(Chart oldChart)
        {
            if(oldChart != null)
                oldChart.TransformChanged -= ParentChart_TransformChanged;
        }

        void ParentChart_TransformChanged(object sender, EventArgs e)
        {
            OnChartTransformChanged();
        }

        protected virtual void OnChartTransformChanged()
        {
        }

        void SetParentChartBinding()
        {
            Binding chartBinding = new Binding();
            chartBinding.RelativeSource = new RelativeSource();
            chartBinding.RelativeSource.Mode = RelativeSourceMode.FindAncestor;
            chartBinding.RelativeSource.AncestorType = typeof(Chart);
            this.SetBinding(ParentChartProperty, chartBinding);
        }
        private Chart FindParentChart()
        {
            System.Windows.FrameworkElement ctrlParent = this;
            while ((ctrlParent = (System.Windows.FrameworkElement)ctrlParent.Parent) != null)
            {
                if (ctrlParent is Chart)
                {
                    return (Chart)ctrlParent;
                }
            }
            return null;
        }
    }
}
