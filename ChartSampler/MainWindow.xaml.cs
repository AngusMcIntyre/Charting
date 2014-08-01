using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using chart = Gusdor.Charting;

namespace ChartSampler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var rand = new Random();

            //chart.Items.Add(GetSeries(p => Math.Sqrt(p), 0, 2000));
            chart.Items.Add(GetSeries(p => (Math.Sin(p)*5)+50, 0, 500, 0.25));
        }

        chart.LineSeries GetSeries(Func<double, double> selection, double start, double count, double increment = 1)
        {
            //Gusdor.Charting.ChartMouseBehaviour
            List<Point?> points = new List<Point?>();

            for (double i = start; i < count; i+=increment)
            {
                points.Add(new Point?(new Point(i, selection(i))));
            }

            return new Gusdor.Charting.LineSeries(points)
                {
                     ProximityCullingAggression = 1                     
                };
        }
    }
}
