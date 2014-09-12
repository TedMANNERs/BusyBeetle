using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BusyBeetle.Client.Properties;
using BusyBeetle.Core;

namespace BusyBeetle.Client
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Service _service = new Service();
        private int _appId;
        private PropertyInfo _selectedColor;

        public MainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            Application.Current.MainWindow.Closing += MainWindowClosing;
            Coordinator = new Coordinator();
            _service.OnAppIdReceivedHandler += AppIdReceived;
            new Task(() => _service.Start("localhost", 6006, Coordinator)).Start();
        }

        public ICommand AddBeetleCommand { get; set; }

        public PropertyInfo SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                OnPropertyChanged();
            }
        }

        public Point SelectedPosition { get; set; }
        public Coordinator Coordinator { get; set; }

        public int AppId
        {
            get { return _appId; }
            set
            {
                _appId = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void AppIdReceived(object sender, AppIdReceivedEventArgs e)
        {
            AppId = e.AppId;
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            Coordinator.World.Stop();
            foreach (Beetle beetle in Coordinator.World.Beetles)
            {
                beetle.Stop();
            }
            _service.Stop();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}