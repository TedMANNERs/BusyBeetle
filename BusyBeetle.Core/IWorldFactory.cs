namespace BusyBeetle.Core
{
    public interface IWorldFactory
    {
        IWorld Create(int width, int height);
    }
}