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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gusdor.Charting;
using System.Globalization;
using System.ComponentModel;

namespace Gusdor.Charting
{
    [TemplatePart(Name = "PART_TickHost", Type = typeof(TickHost))]
    [TemplatePart(Name = "PART_LabelHost", Type = typeof(ContentControl))]
    /// <summary>
    /// Interaction logic for Axis.xaml
    /// </summary>
    public partial class Axis : Control
    {
        static Axis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Axis), new FrameworkPropertyMetadata(typeof(Axis)));
        }

        internal const int MinLabelGap = 5;
        ContentControl m_LabelHost = null;
        TickHost m_TickHost = null;

        public Pen LinePen
        {
            get { return (Pen)GetValue(LinePenProperty); }
            set { SetValue(LinePenProperty, value); }
        }
        public object Label
        {
            get { return (object)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        #region RangeProperty

        public static Range GetRange(DependencyObject obj)
        {
            return (Range)obj.GetValue(RangeProperty);
        }

        public static void SetRange(DependencyObject obj, Range value)
        {
            obj.SetValue(RangeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Range.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.RegisterAttached("Range", typeof(Range), typeof(Axis), new FrameworkPropertyMetadata(new Range(0, 100), FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(object), typeof(Axis), new PropertyMetadata("Axis"));      


        // Using a DependencyProperty as the backing store for LinePen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinePenProperty =
            DependencyProperty.Register("LinePen", typeof(Pen), typeof(Axis), new PropertyMetadata(new Pen(Brushes.Black, 1)));

        #region MinorTickCountProperty
        public int MinorTickCount
        {
            get { return (int)GetValue(MinorTickCountProperty); }
            set { SetValue(MinorTickCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickCountProperty =
            DependencyProperty.Register("MinorTickCount", typeof(int), typeof(Axis), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region MajorTickLengthProperty
        public double MajorTickLength
        {
            get { return (double)GetValue(MajorTickLengthProperty); }
            set { SetValue(MajorTickLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickLengthProperty =
            DependencyProperty.Register("MajorTickLength", typeof(double), typeof(Axis), new FrameworkPropertyMetadata(5.0));
        #endregion

        #region MinorTickLengthProperty
        public double MinorTickLength
        {
            get { return (double)GetValue(MinorTickLengthProperty); }
            set { SetValue(MinorTickLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickLengthProperty =
            DependencyProperty.Register("MinorTickLength", typeof(double), typeof(Axis), new FrameworkPropertyMetadata(3.0, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region LabelPaddingProperty
        public double LabelPadding
        {
            get { return (double)GetValue(LabelPaddingProperty); }
            set { SetValue(LabelPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelPaddingProperty =
            DependencyProperty.Register("LabelPadding", typeof(double), typeof(Axis), new PropertyMetadata(0.0));
        #endregion

        #region OrientationProperty
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Axis), new PropertyMetadata(Orientation.Horizontal));
        #endregion

        #region FlipLabelProperty
        /// <summary>
        /// Gets or sets if the label and ticks are rendered the other way around.
        /// </summary>
        public bool FlipLabel
        {
            get { return (bool)GetValue(FlipLabelProperty); }
            set { SetValue(FlipLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlipLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlipLabelProperty =
            DependencyProperty.Register("FlipLabel", typeof(bool), typeof(Axis), new PropertyMetadata(false));
        #endregion

        #region AxisProviderProperty
        public AxisProvider AxisProvider
        {
            get { return (AxisProvider)GetValue(AxisProviderProperty); }
            set { SetValue(AxisProviderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisProviderProperty =
            DependencyProperty.Register("AxisProvider", typeof(AxisProvider), typeof(Axis), new PropertyMetadata(new NumericAxisProvider()));
        #endregion

        #region RangeStartProperty
        public double RangeStart
        {
            get { return (double)GetValue(RangeStartProperty); }
            set { SetValue(RangeStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeStartProperty =
            DependencyProperty.Register("RangeStart", typeof(double), typeof(Axis), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region RangeEndProperty
        public double RangeEnd
        {
            get { return (double)GetValue(RangeEndProperty); }
            set { SetValue(RangeEndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeEndProperty =
            DependencyProperty.Register("RangeEnd", typeof(double), typeof(Axis), new FrameworkPropertyMetadata(100.1, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        public TickList Ticks
        {
            get { return (TickList)GetValue(TicksPropertyKey.DependencyProperty); }
            private set { SetValue(TicksPropertyKey, value); }
        }        // Using a DependencyProperty as the backing store for Ticks.  This enables animation, styling, binding, etc...
        static readonly DependencyPropertyKey TicksPropertyKey =
            DependencyProperty.RegisterReadOnly("Ticks", typeof(TickList), typeof(Axis), new PropertyMetadata(new TickList()));
        public Axis()
        {
            //Binding b = new Binding();
            //b.Path = new PropertyPath(RangeProperty);
            //b.Converter = new RangePosConverter();
            //b.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            //this.SetBinding(RangeStartProperty, b);

            //b = new Binding();
            //b.Path = new PropertyPath(RangeProperty);
            //b.Converter = new RangePosConverter { GetRangeEnd = true };
            //b.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            //this.SetBinding(RangeEndProperty, b);
        }

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_LabelHost = this.GetTemplateChild("PART_LabelHost") as ContentControl;
            m_TickHost = this.GetTemplateChild("PART_TickHost") as TickHost;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (m_TickHost != null)
            {
                Ticks = CalculateTicks(GetDrawingArgs());
                m_TickHost.InvalidateVisual();
            }
        }

        internal AxisDrawingArgs GetDrawingArgs()
        {
            return new AxisDrawingArgs
            {
                MinorTickCount = this.MinorTickCount,
                Range = new Range(RangeStart, RangeEnd),
                AxisOrientation = this.Orientation,

                LabelColour = this.Foreground,
                LabelFontSize = this.FontSize,
                LabelFontFamily = this.FontFamily,
                LabelFontWeight = this.FontWeight,
                LabelFontStyle = this.FontFamily
            };
        }
        #endregion

        #region "Rendering"

        #endregion

        /// <summary>
        /// Uses the Axis' AxisProvider to calculate and return a new TickList.
        /// </summary>
        TickList CalculateTicks(AxisDrawingArgs args)
        {
            if (this.AxisProvider == null) return new TickList();
            this.AxisProvider.CalculateTicks(args.Range, args);
            return this.AxisProvider.Ticks;
        }

        class RangePosConverter: System.Windows.Data.IValueConverter
        {
            /// <summary>
            /// Gets or sets if the converter should provide the range end. If false, provides the start.
            /// </summary>
            public bool GetRangeEnd { get; set; }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is Range)
                {
                    if(GetRangeEnd)
                        return (value as Range).End;

                    return (value as Range).Start;
                }

                return Binding.DoNothing;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
