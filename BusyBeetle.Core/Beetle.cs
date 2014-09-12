using System.Drawing;
using System.Threading;

namespace BusyBeetle.Core
{
    public class Beetle
    {
        private readonly IWorld _world;
        private bool _isRunning = true;

        public Beetle(int posX, int posY, Color color, IWorld world)
        {
            PositionX = posX;
            PositionY = posY;
            Direction = 0;
            Color = color;
            _world = world;
        }

        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public Direction Direction { get; set; }
        public Color Color { get; set; }
        public PixelData ModifiedPixel { get; private set; }

        public void TurnLeft()
        {
            Direction = Direction.Previous();
        }

        public void TurnRight()
        {
            Direction = Direction.Next();
        }

        public void MoveStraight()
        {
            switch (Direction)
            {
                case Direction.Up:
                    PositionY++;
                    break;
                case Direction.Right:
                    PositionX++;
                    break;
                case Direction.Down:
                    PositionY--;
                    break;
                case Direction.Left:
                    PositionX--;
                    break;
            }
        }

        public void Update()
        {
            do
            {
                UpdateColorAndDirection();
                MoveStraight();
                CheckBorder();
                Thread.Sleep(1);
            }
            while (_isRunning && !Connection.IsEstablished);
        }

        public void CheckBorder()
        {
            if (PositionX >= _world.Width)
                PositionX = 0;
            else if (PositionX < 0)
                PositionX = _world.Width - 1;

            if (PositionY >= _world.Height)
                PositionY = 0;
            else if (PositionY < 0)
                PositionY = _world.Height - 1;
        }

        public void UpdateColorAndDirection()
        {
            if (_world.GetAt(PositionX, PositionY).ToArgb() == Color.ToArgb())
            {
                _world.SetAt(PositionX, PositionY, Color.White);
                ModifiedPixel = new PixelData(PositionX, PositionY, Color.White);
                TurnLeft();
            }
            else
            {
                _world.SetAt(PositionX, PositionY, Color);
                ModifiedPixel = new PixelData(PositionX, PositionY, Color);
                TurnRight();
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}