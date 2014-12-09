using System;
using System.Windows.Threading;

namespace BusyBeetle.Core
{
    public class BeetleDispatcher : IDispatcher
    {
        private readonly Dispatcher _dispatcher;

        public BeetleDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public T Invoke<T>(Func<T> action)
        {
            return _dispatcher.Invoke(action);
        }

        public void BeginInvoke(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }
    }
}