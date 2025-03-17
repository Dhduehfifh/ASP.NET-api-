using System;
using System.Collections.Generic;

namespace Database
{
    /// <summary>
    /// 核心数据库管理器，支持动态 ORM 表映射与数据库配置
    /// </summary>
    public class DatabaseCore
    {
        /// <summary>
        /// 当前数据库逻辑名称
        /// </summary>
        private string _dbName;

        /// <summary>
        /// 表映射集合
        /// </summary>
        private Dictionary<string, OrmMapping> _tableMappings;

        /// <summary>
        /// 数据库配置（连接信息等），调用 SetConfig 设置
        /// </summary>
        private DBConfig _config;

        /// <summary>
        /// 构造器，初始化数据库名和表映射
        /// </summary>
        public DatabaseCore(string dbName, Dictionary<string, OrmMapping> tableMappings)
        {
            _dbName = dbName;
            _tableMappings = tableMappings;
        }

        /// <summary>
        /// 动态注入数据库配置
        /// </summary>
        public void SetConfig(DBConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 获取 ORM 映射配置
        /// </summary>
        public OrmMapping GetOrmMapping(string logicalName)
        {
            if (_tableMappings.ContainsKey(logicalName))
                return _tableMappings[logicalName];
            throw new Exception($"未找到逻辑表映射：{logicalName}");
        }

        /// <summary>
        /// 动态解析表名（支持分表等）
        /// </summary>
        public string ResolveTable(string logicalName, object contextData = null)
        {
            var mapping = GetOrmMapping(logicalName);
            return mapping.ResolveTable(contextData);
        }

        /// <summary>
        /// 获取当前数据库配置
        /// </summary>
        public DBConfig GetConfig() => _config;

        /// <summary>
        /// 获取数据库逻辑名称
        /// </summary>
        public string GetDatabaseName() => _dbName;

        // 后续：SaveJson、Query、Update、Delete 接口预留
    }
}