using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Database
{
    public class DatabaseCore
    {
        private string _dbName;
        private Dictionary<string, OrmMapping> _tableMappings;
        private DBConfig _config;

        // 现在 DBConfig 作为参数传递
        public DatabaseCore(string dbName, Dictionary<string, OrmMapping> tableMappings, DBConfig config)
        {
            _dbName = dbName;
            _tableMappings = tableMappings;
            _config = config;
        }

        public DBConfig GetConfig() => _config;

        public OrmMapping GetOrmMapping(string logicalName)
        {
            if (_tableMappings.TryGetValue(logicalName, out var mapping))
                return mapping;
            throw new Exception($"未找到逻辑表映射: {logicalName}");
        }

        public string ResolveTable(string logicalName, object contextData = null)
        {
            return GetOrmMapping(logicalName).ResolveTable(contextData);
        }

        public void Save(Dictionary<string, object> fields, string logicalName, DbContext context)
        {
            var table = ResolveTable(logicalName, fields);
            var sqlLizer = SqlLizerPool.Rent();

            try
            {
                var sql = sqlLizer.BuildInsert(table, fields);
                var adapter = context.Config.GetSqlAdapter();
                adapter.Execute(sql, fields); // 参数绑定执行
            }
            finally
            {
                SqlLizerPool.Return(sqlLizer);
            }
        }
    }
}