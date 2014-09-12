using System;
using System.ComponentModel;

namespace BusyBeetle.Core
{
    public static class Connection
    {
        private static bool _isEstablished;

        public static bool IsEstablished
        {
            get { return _isEstablished; }
            set
            {
                _isEstablished = value;
                OnStaticPropertyChanged("IsEstablished");
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