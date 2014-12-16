namespace BusyBeetle.Core
{
    public interface IService
    {
        void Init(IConfiguration config);

        void Start();

        void Stop();
    }
}