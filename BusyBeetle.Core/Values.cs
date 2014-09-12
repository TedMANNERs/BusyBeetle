using System;
using System.ComponentModel;

namespace BusyBeetle.Core
{
    public static class Values
    {
        private static double _scalefactor = 1;

        public static double Scalefactor
        {
            get { return _scalefactor; }
            set
            {
                _scalefactor = value; 
                OnStaticPropertyChanged("Scalefactor");
            }
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static void OnStaticPropertyChanged(string staticPropertyName)
        {
            if (StaticPropertyChanged != null)
                StaticPropertyChanged(null, new PropertyChangedEventArgs(staticPropertyName));
        }
    }
}