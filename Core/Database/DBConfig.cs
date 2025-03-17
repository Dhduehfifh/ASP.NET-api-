using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Database
{
    public class DBConfig
    {
        public string DatabaseType { get; set; }
        public string ConnectionString { get; set; }

        // 预留扩展：动态适配不同 SQL 构建器
        public ISqlAdapter GetSqlAdapter()
        {
            // 这里简单示例，实际按 DatabaseType 返回不同适配器
            return new MySqlAdapter(ConnectionString);
        }
        
    }

    public class ISqlAdapter
    {
        //暂且作为空实现
    }
    public class MySqlAdapter
    {
        public string _connectionString { get; private set; }
        public MySqlAdapter(string ConnectionString)
        {
            _connectionString = ConnectionString;
        }
    }
}