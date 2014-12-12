using System;

namespace BusyBeetle.Core.Direction
{
    public static class DirectionExtensions
    {
        public static Direction Next(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Up;
            }
            throw new ArgumentException("direction");
        }

        public static Direction Previous(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Up;
            }
            throw new ArgumentException("direction");
        }
    }
}