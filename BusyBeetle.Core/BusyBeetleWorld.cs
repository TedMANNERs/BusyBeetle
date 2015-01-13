using System.Collections.Generic;
using System.Drawing;

namespace BusyBeetle.Core
{
    public class BusyBeetleWorld : World
    {
        public BusyBeetleWorld(int width, int height)
            : base(width, height)
        {
        }

        public override GameType GameType
        {
            get { return GameType.BusyBeetle; }
        }

        public override List<PixelData> Tick()
        {
            List<PixelData> modifiedPixels = new List<PixelData>();

            foreach (Beetle beetle in Beetles)
            {
                Color updatedColor = beetle.UpdateColorAndDirection(GetAt(beetle.PositionX, beetle.PositionY));
                SetAt(beetle.PositionX, beetle.PositionY, updatedColor);
                beetle.MoveStraight();
                beetle.ClampPosition(Width, Height);
                modifiedPixels.Add(beetle.ModifiedPixel);
            }
            return modifiedPixels;
        }
    }
}