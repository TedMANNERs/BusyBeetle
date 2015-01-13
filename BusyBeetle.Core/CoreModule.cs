using BusyBeetle.Core.Serialization;
using Ninject.Modules;

namespace BusyBeetle.Core
{
    internal class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IWorldFactory>().To<WorldFactory>();
            Bind<ISerializer>().To<Serializer>();
            Bind<ICoordinator>().To<Coordinator>().InSingletonScope();
        }
    }
}