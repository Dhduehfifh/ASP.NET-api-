using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.ObjectPool;

namespace Core.Database
{
    public class SqlLizer
    {
        public string BuildInsert(string tableName, Dictionary<string, object> fields)
        {
            var columns = string.Join(",", fields.Keys);
            var values = string.Join(",", fields.Keys.Select(k => "@" + k));
            return $"INSERT INTO {tableName} ({columns}) VALUES ({values});";
        }

        public void Reset()
        {
            // 预留未来缓存清理
        }
    }

    public static class SqlLizerPool
    {
        private static readonly ObjectPool<SqlLizer> _pool = PoolFactory.CreatePool<SqlLizer>(200);
        public static SqlLizer Rent() => _pool.Get();
        public static void Return(SqlLizer obj) => _pool.Return(obj);
    }
}