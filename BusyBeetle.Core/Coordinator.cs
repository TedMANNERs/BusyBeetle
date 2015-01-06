using System;
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
            World = worldFactory.Create(200, 200, true);
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
                        //Beetle beetle = new Beetle((int)(position.X / Values.Scalefactor), (int)(position.Y / Values.Scalefactor), Color.FromArgb(color.A, color.R, color.G, color.B));
                        //World.Beetles.Add(beetle);
                        Random r = new Random();

                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                if (r.Next(2) == 0)
                                    World.SetAt((int)(position.X / Values.Scalefactor) + i, (int)(position.Y / Values.Scalefactor) + j, Color.Black);
                            }
                        }

                        //for (int i = 0; i < World.Width; i += 2)
                        //{
                        //    World.Beetles.Add(new Beetle(i, 50, Color.FromArgb(i * 5 % 255, i * 5 % 255, i * 5 % 255)));
                        //}
                    }
                });
            BeetleTasks.Add(task);
            task.Start();
        }
    }
}