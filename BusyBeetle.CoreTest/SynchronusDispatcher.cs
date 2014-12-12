using System;
using BusyBeetle.Core;
using BusyBeetle.Core.Dispatcher;

namespace BusyBeetle.CoreTest
{
    public class SynchronusDispatcher : IDispatcher
    {
        public T Invoke<T>(Func<T> action)
        {
            return action();
        }

        public void BeginInvoke(Action action)
        {
            action();
        }
    }
}