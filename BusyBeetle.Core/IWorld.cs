using System.Drawing;

namespace BusyBeetle.Core
{
    public interface IWorld
    {
        int Height { get; set; }
        int Width { get; set; }

        Color GetAt(int x, int y);

        void SetAt(int x, int y, Color color);
    }
}