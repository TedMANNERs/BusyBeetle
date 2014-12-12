using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BusyBeetle.Core.Dispatcher;
using BusyBeetle.Core.Properties;

namespace BusyBeetle.Core
{
    public class World : INotifyPropertyChanged, IWorld
    {
        private readonly IDispatcher _dispatcher;
        private bool _isRunning = true;

        public World(IDispatcher dispatcher, int width, int height)
        {
            Width = width;
            Height = height;
            _dispatcher = dispatcher;
            Beetles = new List<Beetle>();
            CreateBitmap();
            Task updateTask = new Task(Update);
            updateTask.Start();
        }

        public Bitmap Bitmap { get; set; }
        public List<Beetle> Beetles { get; set; }

        public int HeightScaled
        {
            get { return (int)(Height * Values.Scalefactor); }
        }

        public int WidthScaled
        {
            get { return (int)(Width * Values.Scalefactor); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public int Height { get; set; }
        public int Width { get; set; }

        public Color GetAt(int x, int y)
        {
            try
            {
                return _dispatcher.Invoke(() => Bitmap.GetPixel(x, y));
            }
            catch (TaskCanceledException)
            {
                return new Color();
            }
        }

        public void SetAt(int x, int y, Color color)
        {
            try
            {
                _dispatcher.BeginInvoke(() => Bitmap.SetPixel(x, y, color));
            }
            catch (TaskCanceledException)
            {
            }
        }

        private void CreateBitmap()
        {
            Bitmap = new Bitmap(Width, Height);

            for (int x = 0; x < Bitmap.Width; x++)
            {
                for (int y = 0; y < Bitmap.Height; y++)
                {
                    Bitmap.SetPixel(x, y, Color.White);
                }
            }
        }

        private void Update()
        {
            while (_isRunning)
            {
                OnPropertyChanged("Bitmap");
                OnPropertyChanged("HeightScaled");
                OnPropertyChanged("WidthScaled");
                Thread.Sleep(20);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}