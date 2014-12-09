using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace BusyBeetle.Core
{
    internal class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IWorldFactory>().ToFactory();
            Bind<IWorld>().To<World>();
        }
    }
}