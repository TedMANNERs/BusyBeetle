using System.ComponentModel;
using System.Runtime.CompilerServices;
using BusyBeetle.Core.Properties;

namespace BusyBeetle.Core
{
    public class Coordinator : ICoordinator, INotifyPropertyChanged
    {
        private IWorld _world;

        public IWorld World
        {
            get { return _world; }
            set
            {
                _world = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}