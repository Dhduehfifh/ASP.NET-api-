using System;
using System.Collections.Generic;
using Toolbox;

namespace Database
{
    /// <summary>
    /// 数据库核心管理器，支持动态 ORM 映射、配置、池化适配
    /// </summary>
    public class DatabaseCore
    {
        private string _dbName;
        private Dictionary<string, OrmMapping> _tableMappings;
        private DBConfig _config;

        /// <summary>
        /// 构造器，创建数据库实例，绑定数据库名和表映射
        /// </summary>
        public DatabaseCore(string dbName, Dictionary<string, OrmMapping> tableMappings)
        {
            _dbName = dbName;
            _tableMappings = tableMappings;
        }

        /// <summary>
        /// 注入数据库配置
        /// </summary>
        public void SetConfig(DBConfig config) => _config = config;

        /// <summary>
        /// 动态增加/覆盖逻辑表映射
        /// </summary>
        public void AddMapping(string logicalName, string realTable)
        {
            _tableMappings[logicalName] = new OrmMapping
            {
                LogicalName = logicalName,
                PhysicalTable = realTable
            };
        }

        /// <summary>
        /// 获取 ORM 映射
        /// </summary>
        public OrmMapping GetOrmMapping(string logicalName)
        {
            if (_tableMappings.TryGetValue(logicalName, out var mapping))
                return mapping;
            throw new Exception($"未找到逻辑表映射: {logicalName}");
        }

        /// <summary>
        /// 动态解析表名（支持分表）
        /// </summary>
        public string ResolveTable(string logicalName, object contextData = null)
        {
            return GetOrmMapping(logicalName).ResolveTable(contextData);
        }

        /// <summary>
        /// 获取数据库逻辑名
        /// </summary>
        public string GetDatabaseName() => _dbName;

        /// <summary>
        /// 获取数据库配置
        /// </summary>
        public DBConfig GetConfig() => _config;

        /// <summary>
        /// 数据持久化（Insert 示例），未来可扩展为 Update/Delete
        /// </summary>
        public void SaveJson(Jsonfier obj, DbContext context)
        {
            var sqlLizer = context.Config.UsePool ? SqlLizerPoolAdapter.Rent() : new SqlLizer(); // 池化适配

            try
            {
                var sql = sqlLizer.BuildInsert(obj);
                var adapter = context.Config.GetSqlAdapter();
                adapter.Execute(sql, obj.Fields); // 参数化执行
            }
            finally
            {
                if (context.Config.UsePool) SqlLizerPoolAdapter.Return(sqlLizer);
            }
        }
    }
}