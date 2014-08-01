using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Gusdor.Charting
{
    /// <summary>
    /// Single axis Transform
    /// </summary>
    public class SingleAxisTransform: INotifyPropertyChanged
    {
        public event EventHandler TransformChanged;

        #region LineScale
        private double m_Scale = 1.0;
        public double Scale
        {
            get { return m_Scale; }
            set 
            { 
                m_Scale = value;
                if(this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Scale"));

                if (this.TransformChanged != null)
                    this.TransformChanged(this, new System.EventArgs());
            }
        }
        #endregion
        
        #region Offset
        private double m_Offset = 0.0;
        public double Offset
        {
            get { return m_Offset; }
            set 
            { 
                m_Offset = value;
                if(this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Offset"));

                if (this.TransformChanged != null)
                    this.TransformChanged(this, new System.EventArgs());
            }
        }
        #endregion        

        public double Transform(double value)
        {
            return SingleAxisTransform.InverseTransform(value, this.Offset, this.Scale);
        }

        public double InverseTransform(double value)
        {
            return SingleAxisTransform.Transform(value, this.Offset, this.Scale);
        }

        public static double Transform(double value, double offset, double scale)
        {
            return (value * scale) + offset;
        }

        public static double InverseTransform(double value, double offset, double scale)
        {
            return (value - offset) / scale;
        }
        
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
