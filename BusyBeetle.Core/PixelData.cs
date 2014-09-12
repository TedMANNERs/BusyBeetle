using System;
using System.Drawing;

namespace BusyBeetle.Core
{
    [Serializable]
    public class PixelData
    {
        public PixelData(int posX, int posY, Color color)
        {
            PositionX = posX;
            PositionY = posY;
            Color = color;
        }

        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public Color Color { get; set; }
    }
}