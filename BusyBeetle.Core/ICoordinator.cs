using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace BusyBeetle.Core
{
    public interface ICoordinator
    {
        IWorld World { get; set; }
        IList<Task> BeetleTasks { get; set; }

        void SpawnAt(Point position, System.Windows.Media.Color color);
    }
}