using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace BusyBeetle.Core
{
    public interface ICoordinator
    {
        IWorld World { get; }
        IList<Task> BeetleTasks { get; set; }

        void SpawnBeetleAt(Point position, System.Windows.Media.Color color);

        void CreateWorld(int width, int height);
    }
}