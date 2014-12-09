﻿using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
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

            AddBeetleCommand = new DelegateCommand(obj => Coordinator.SpawnBeetleAt(Mouse.GetPosition((IInputElement)obj), (Color)SelectedColor.GetValue(null)), () => true);
            GetColorCommand = new DelegateCommand(obj => GetPixelColor(Mouse.GetPosition((IInputElement)obj)), () => true);
            _service.OnAppIdReceivedHandler += AppIdReceived;

            IDispatcher dispatcher = new BeetleDispatcher(Dispatcher.CurrentDispatcher);
            CoreKernel.Instance.Kernel.Bind<IDispatcher>().ToConstant(dispatcher).InSingletonScope();
            Coordinator = CoreKernel.Get<Coordinator>();

            new Task(() => _service.Start("ch10dd279", 6006, Coordinator)).Start();
        }

        public ICommand AddBeetleCommand { get; set; }
        public ICommand GetColorCommand { get; set; }

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

        private void GetPixelColor(Point mousePosition)
        {
            System.Drawing.Color color = Coordinator.World.GetAt((int)(mousePosition.X / Values.Scalefactor), (int)(mousePosition.Y / Values.Scalefactor));
            SelectedColor = typeof(Colors).GetProperties().FirstOrDefault(p => System.Drawing.Color.FromName(p.Name).ToArgb() == color.ToArgb());
        }

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