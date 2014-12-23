using System.Drawing;
using BusyBeetle.Core.Direction;

namespace BusyBeetle.Core
{
    public class Beetle
    {
        public Beetle(int posX, int posY, Color color)
        {
            PositionX = posX;
            PositionY = posY;
            Direction = 0;
            Color = color;
        }

        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public Direction.Direction Direction { get; set; }
        public Color Color { get; set; }
        public PixelData ModifiedPixel { get; set; }

        public void TurnLeft()
        {
            Direction = Direction.Previous();
        }

        public void TurnRight()
        {
            Direction = Direction.Next();
        }

        public void ClampPosition(int width, int height)
        {
            if (PositionX >= width)
                PositionX = 0;
            else if (PositionX < 0)
                PositionX = width - 1;

            if (PositionY >= height)
                PositionY = 0;
            else if (PositionY < 0)
                PositionY = height - 1;
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

        public Color UpdateColorAndDirection(Color currentColor)
        {
            if (currentColor.ToArgb() == Color.ToArgb())
            {
                ModifiedPixel = new PixelData(PositionX, PositionY, Color.White);
                TurnLeft();
                return Color.White;
            }

            ModifiedPixel = new PixelData(PositionX, PositionY, Color);
            TurnRight();
            return Color;
        }
    }
}