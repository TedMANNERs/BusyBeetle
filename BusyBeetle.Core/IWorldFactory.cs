using System;

namespace BusyBeetle.Core
{
    public interface IWorldFactory
    {
        IWorld Create(int width, int height, GameType gameType);
    }

    public class WorldFactory : IWorldFactory
    {
        public IWorld Create(int width, int height, GameType gameType)
        {
            switch (gameType)
            {
                case GameType.BusyBeetle:
                    return new BusyBeetleWorld(width, height);
                case GameType.GameOfLife:
                    return new GameOfLifeWorld(width, height);
                default:
                    throw new ArgumentOutOfRangeException("gameType");
            }
        }
    }
}