using System;

namespace BusyBeetle.Core.Dispatcher
{
    public class BeetleDispatcher : IDispatcher
    {
        private readonly System.Windows.Threading.Dispatcher _dispatcher;

        public BeetleDispatcher(System.Windows.Threading.Dispatcher dispatcher)
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