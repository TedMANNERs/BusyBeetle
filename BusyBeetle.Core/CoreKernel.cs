using System;
using Ninject;

namespace BusyBeetle.Core
{
    public class CoreKernel : IDisposable
    {
        private static CoreKernel _instance;

        private CoreKernel()
        {
            Kernel = new StandardKernel();
            Kernel.Load<CoreModule>();
        }

        public static CoreKernel Instance
        {
            get { return _instance ?? (_instance = new CoreKernel()); }
        }

        public IKernel Kernel { get; private set; }

        public static T Get<T>()
        {
            return Instance.Kernel.Get<T>();
        }

        public static void ClearInstance()
        {
            if (_instance != null)
            {
                _instance.Dispose();
                _instance = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Kernel != null)
                {
                    Kernel.Dispose();
                    Kernel = null;
                }
            }
        }
    }
}