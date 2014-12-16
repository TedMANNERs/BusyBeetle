using System.Windows;

namespace BusyBeetle.Core
{
    public interface ICoordinator
    {
        IWorld World { get; }

        void SpawnBeetleAt(Point position, System.Windows.Media.Color color);
    }
}