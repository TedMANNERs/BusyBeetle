using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BusyBeetle.Core.Properties;

namespace BusyBeetle.Core
{
    public abstract class World : INotifyPropertyChanged, IWorld, IDisposable
    {
        private readonly List<Point> _modifiedPixels = new List<Point>();
        private readonly Task _updateTask;
        private bool _isRunning = true;
        private Color[][] _pixelArray;

        protected World(int width, int height)
        {
            Width = width;
            Height = height;
            Beetles = new List<Beetle>();
            CreateBitmap();
            InitPixelArray(width, height);
            LockObject = new object();

            _updateTask = new Task(Update);
            _updateTask.Start();
        }

        public static object LockObject { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public int HeightScaled
        {
            get { return (int)(Height * Values.Scalefactor); }
        }

        public int WidthScaled
        {
            get { return (int)(Width * Values.Scalefactor); }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Bitmap Bitmap { get; set; }
        public List<Beetle> Beetles { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public abstract GameType GameType { get; }

        public Color GetAt(int x, int y)
        {
            return _pixelArray[x][y];
        }

        public void SetAt(int x, int y, Color color)
        {
            _pixelArray[x][y] = color;
            lock (_modifiedPixels)
            {
                _modifiedPixels.Add(new Point(x, y));
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _updateTask.Wait();
            _updateTask.Dispose();
        }

        public void SetNewSize(int width, int height)
        {
            Bitmap = new Bitmap(width, height);
            Width = width;
            Height = height;
            InitPixelArray(width, height);
        }

        public abstract List<PixelData> Tick();

        private void InitPixelArray(int width, int height)
        {
            _pixelArray = new Color[width][];

            for (int i = 0; i < width; i++)
            {
                _pixelArray[i] = new Color[height];
                for (int j = 0; j < height; j++)
                {
                    _pixelArray[i][j] = Color.White;
                }
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
                lock (_modifiedPixels)
                {
                    foreach (Point pixel in _modifiedPixels)
                    {
                        lock (LockObject)
                        {
                            Bitmap.SetPixel(pixel.X, pixel.Y, _pixelArray[pixel.X][pixel.Y]);
                        }
                    }
                    _modifiedPixels.Clear();
                }
                OnPropertyChanged(null);
                Thread.Sleep(10);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateTask.Dispose();
            }
        }
    }
}