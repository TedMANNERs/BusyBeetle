using System.Drawing;
using BusyBeetle.Core.Direction;

namespace BusyBeetle.Core
{
    public class Beetle
    {
        private readonly IWorld _world;

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
        public Direction.Direction Direction { get; set; }
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
                case Core.Direction.Direction.Up:
                    PositionY++;
                    break;
                case Core.Direction.Direction.Right:
                    PositionX++;
                    break;
                case Core.Direction.Direction.Down:
                    PositionY--;
                    break;
                case Core.Direction.Direction.Left:
                    PositionX--;
                    break;
            }
        }

        public void Tick()
        {
            UpdateColorAndDirection();
            MoveStraight();
            CheckBorder();
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
    }
}