using System;

namespace BusyBeetle.Core.Dispatcher
{
    public interface IDispatcher
    {
        T Invoke<T>(Func<T> action);
        void BeginInvoke(Action action);
    }
}