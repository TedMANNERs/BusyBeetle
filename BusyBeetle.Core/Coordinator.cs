using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Point = System.Windows.Point;

namespace BusyBeetle.Core
{
    public class Coordinator : ICoordinator
    {
        public Coordinator(IWorldFactory worldFactory)
        {
            World = worldFactory.Create((int)(200 * Values.Scalefactor), (int)(200 * Values.Scalefactor), true);
            BeetleTasks = new List<Task>();
        }

        public IList<Task> BeetleTasks { get; set; }
        public IWorld World { get; private set; }

        public void SpawnBeetleAt(Point position, System.Windows.Media.Color color)
        {
            Task task = new Task(
                () =>
                {
                    lock (World.Beetles)
                    {
                        Beetle beetle = new Beetle((int)(position.X / Values.Scalefactor), (int)(position.Y / Values.Scalefactor), Color.FromArgb(color.A, color.R, color.G, color.B), World);
                        World.Beetles.Add(beetle);
                    }
                });
            BeetleTasks.Add(task);
            task.Start();
        }
    }
}