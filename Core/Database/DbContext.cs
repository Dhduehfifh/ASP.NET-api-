using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Database
{
    public interface IResettable
    {
        void Reset(); // 归还池时自动重置
    }

    public class DbContext : IResettable
    {
        public string TableName { get; set; }
        public DBConfig Config { get; set; }

        public void Reset()
        {
            TableName = null;
            Config = null;
        }
    }

    public static class DbContextPool
    {
        private static readonly ObjectPool<DbContext> _pool = PoolFactory.CreatePool<DbContext>(200);
        public static DbContext Rent() => _pool.Get();
        public static void Return(DbContext obj) => _pool.Return(obj);
    }
}