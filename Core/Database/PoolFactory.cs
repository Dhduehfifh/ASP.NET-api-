using System;
using Microsoft.Extensions.ObjectPool;

namespace Core.Database
{
    public static class PoolFactory
    {
        /// 🌍 **泛型对象池工厂**
        public static ObjectPool<T> CreatePool<T>(int maxSize) where T : class, new()
        {
            return new DefaultObjectPool<T>(new DefaultPooledObjectPolicy<T>(), maxSize);
        }

        /// 🌍 **专门为 `DbContext` 设定的对象池**
        private static readonly ObjectPool<DbContext> _dbContextPoolInstance =
            new DefaultObjectPool<DbContext>(new DbContextPolicy(), DefaultConfig.PoolSize);

        public static DbContext RentDbContext() => _dbContextPoolInstance.Get();
        public static void ReturnDbContext(DbContext dbContext) => _dbContextPoolInstance.Return(dbContext);
    }

    public class DbContextPolicy : PooledObjectPolicy<DbContext>
    {
        public override DbContext Create() => new DbContext();
        public override bool Return(DbContext dbContext)
        {
            dbContext.Dispose();
            return true;
        }
    }
}