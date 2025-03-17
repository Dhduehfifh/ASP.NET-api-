using Microsoft.Extensions.ObjectPool;

namespace Database
{
    public class StandardPolicy<T> : PooledObjectPolicy<T> where T : class, new()
    {
        public override T Create() => new T();

        public override bool Return(T obj)
        {
            if (obj is IResettable resettable)
                resettable.Reset(); // 自动重置
            return true;
        }
    }

    public static class PoolFactory
    {
        public static ObjectPool<T> CreatePool<T>(int maxSize = 200) where T : class, new()
            => new DefaultObjectPool<T>(new StandardPolicy<T>(), maxSize);
    }
}