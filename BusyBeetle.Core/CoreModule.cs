using BusyBeetle.Core.Serialization;
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
            Bind<ISerializer>().To<Serializer>();
            Bind<ICoordinator>().To<Coordinator>().InSingletonScope();
        }
    }
}