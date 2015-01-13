using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BusyBeetle.Core.Properties;
using Point = System.Windows.Point;

namespace BusyBeetle.Core
{
    public class Coordinator : ICoordinator, INotifyPropertyChanged
    {
        private IWorld _world;

        public Coordinator()
        {
            BeetleTasks = new List<Task>();
        }

        public IList<Task> BeetleTasks { get; set; }

        public IWorld World
        {
            get { return _world; }
            set
            {
                _world = value;
                OnPropertyChanged();
            }
        }

        public void SpawnAt(Point position, System.Windows.Media.Color color)
        {
            if (World.GameType == GameType.GameOfLife)
            {
                Random r = new Random();

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (r.Next(2) == 0)
                            World.SetAt((int)(position.X / Values.Scalefactor) + i, (int)(position.Y / Values.Scalefactor) + j, Color.Black);
                    }
                }
            }
            else
            {
                Task task = new Task(
                    () =>
                    {
                        lock (World.Beetles)
                        {
                            Beetle beetle = new Beetle((int)(position.X / Values.Scalefactor), (int)(position.Y / Values.Scalefactor), Color.FromArgb(color.A, color.R, color.G, color.B));
                            World.Beetles.Add(beetle);

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}