using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace BusyBeetle.Core
{
    public interface IWorld
    {
        int Height { get; set; }
        int Width { get; set; }
        List<Beetle> Beetles { get; set; }
        Bitmap Bitmap { get; set; }
        int HeightScaled { get; }
        int WidthScaled { get; }
        GameType GameType { get; }
        IList<Task> BeetleTasks { get; set; }

        Color GetAt(int x, int y);

        void SetAt(int x, int y, Color color);

        void Stop();

        void SetNewSize(int width, int height);

        List<PixelData> Tick();

        void Dispose();

        void SpawnAt(System.Windows.Point position, System.Windows.Media.Color color);
    }
}