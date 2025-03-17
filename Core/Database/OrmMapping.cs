using System;

namespace Database
{
    /// <summary>
    /// ORM 映射对象，支持静态表名和动态分表解析
    /// </summary>
    public class OrmMapping
    {
        /// <summary>
        /// 逻辑名（如 "User"）
        /// </summary>
        public string LogicalName { get; set; }

        /// <summary>
        /// 静态表名（如 "users"，如果不分表直接用这个）
        /// </summary>
        public string PhysicalTable { get; set; }

        /// <summary>
        /// 分表解析器（如需要按日期/用户ID分表）
        /// </summary>
        public Func<object, string> TableResolver { get; set; }

        /// <summary>
        /// 解析最终表名（如果设置了动态分表优先使用）
        /// </summary>
        public string ResolveTable(object contextData = null)
        {
            if (TableResolver != null && contextData != null)
                return TableResolver(contextData);
            return PhysicalTable;
        }
    }
}