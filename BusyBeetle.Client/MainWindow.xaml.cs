using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using BusyBeetle.Core;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace BusyBeetle.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = (MainViewModel)DataContext;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mainViewModel.SelectedPosition = e.MouseDevice.GetPosition(sender as IInputElement);
            _mainViewModel.Coordinator.SpawnBeetleAt(e.MouseDevice.GetPosition(sender as IInputElement), (System.Windows.Media.Color)_mainViewModel.SelectedColor.GetValue(null, null));
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(sender as IInputElement);
            Color color = _mainViewModel.Coordinator.World.GetAt((int)(position.X / Values.Scalefactor), (int)(position.Y / Values.Scalefactor));
            _mainViewModel.SelectedColor = typeof(Colors).GetProperties().FirstOrDefault(p => Color.FromName(p.Name).ToArgb() == color.ToArgb());
        }
    }
}