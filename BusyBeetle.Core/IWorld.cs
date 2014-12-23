using System.Collections.Generic;
using System.Drawing;

namespace BusyBeetle.Core
{
    public interface IWorld
    {
        int Height { get; set; }
        int Width { get; set; }
        List<Beetle> Beetles { get; set; }
        Bitmap Bitmap { get; set; }

        Color GetAt(int x, int y);

        void SetAt(int x, int y, Color color);

        void Stop();

        void SetNewSize(int width, int height);

        List<PixelData> Tick();
    }
}