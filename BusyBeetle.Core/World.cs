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
    public class World : INotifyPropertyChanged, IWorld, IDisposable
    {
        private readonly List<Point> _modifiedPixels = new List<Point>();
        private readonly Task _updateTask;
        private bool _isRunning = true;
        private Color[][] _pixelArray;

        public World(int width, int height, bool updating)
        {
            Width = width;
            Height = height;
            Beetles = new List<Beetle>();
            CreateBitmap();
            InitPixelArray(width, height);
            LockObject = new object();

            if (!updating)
                return;

            _updateTask = new Task(Update);
            _updateTask.Start();
        }

        public int HeightScaled
        {
            get { return (int)(Height * Values.Scalefactor); }
        }

        public int WidthScaled
        {
            get { return (int)(Width * Values.Scalefactor); }
        }

        public static object LockObject { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public Bitmap Bitmap { get; set; }
        public List<Beetle> Beetles { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

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

        public List<PixelData> Tick()
        {
            List<PixelData> modifiedPixels = new List<PixelData>();

            //foreach (Beetle beetle in Beetles)
            //{
            //    Color updatedColor = beetle.UpdateColorAndDirection(GetAt(beetle.PositionX, beetle.PositionY));
            //    SetAt(beetle.PositionX, beetle.PositionY, updatedColor);
            //    beetle.MoveStraight();
            //    beetle.ClampPosition(Width, Height);
            //    modifiedPixels.Add(beetle.ModifiedPixel);
            //}

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    List<PixelData> aliveNeighbours = GetNeighbours(i, j);

                    if (GetAt(i, j).ToArgb() == Color.Black.ToArgb())
                    {
                        if (aliveNeighbours.Count < 2)
                        {
                            //SetAt(i, j, Color.White);
                            modifiedPixels.Add(new PixelData(i, j, Color.White));
                        }
                        else if (aliveNeighbours.Count < 4)
                        {
                            //SetAt(i, j, Color.Black);
                            modifiedPixels.Add(new PixelData(i, j, Color.Black));
                        }
                        else
                        {
                            //SetAt(i, j, Color.White);
                            modifiedPixels.Add(new PixelData(i, j, Color.White));
                        }
                    }
                    else if (aliveNeighbours.Count == 3)
                    {
                        //SetAt(i, j, Color.Black);
                        modifiedPixels.Add(new PixelData(i, j, Color.Black));
                    }
                }
            }

            foreach (PixelData modifiedPixel in modifiedPixels)
                SetAt(modifiedPixel.PositionX, modifiedPixel.PositionY, modifiedPixel.Color);

            return modifiedPixels;
        }

        private List<PixelData> GetNeighbours(int x, int y)
        {
            List<PixelData> neighbours = new List<PixelData>();
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y)
                        continue;

                    Color color = GetAt(Clamp(i), Clamp(j));
                    if (color.ToArgb() == Color.Black.ToArgb())
                    {
                        neighbours.Add(new PixelData(Clamp(i), Clamp(j), color));
                    }
                }
            }
            return neighbours;
        }

        private int Clamp(int i)
        {
            if (i < 0)
                return Width - 1;
            if (i >= Width)
                return 0;
            return i;
        }

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
                OnPropertyChanged("Bitmap");
                OnPropertyChanged("HeightScaled");
                OnPropertyChanged("WidthScaled");
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