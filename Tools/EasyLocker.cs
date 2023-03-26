using System;

namespace Tools
{
    public static class LockEx
    {
        public static EasyLocker<T> CreateLocker<T>(this T instance) where T : class
        {
            return new EasyLocker<T>(instance);
        }
    }
    public class EasyLocker<T> where T : class
    {
        private T Instance;
        private object Lock = new object();
        public EasyLocker(T instance)
        {
            Instance = instance;
        }

        public void Execute(Action<T> action)
        {
            lock (Lock)
            {
                action(Instance);
            }
        }
        public R Execute<R>(Func<T, R> func)
        {
            lock (Lock)
            {
                return func(Instance);
            }
        }

        public void Update(T instance)
        {
            lock (Lock)
            {
                Instance = instance;
            }
        }
    }
}
