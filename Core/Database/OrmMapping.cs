using System;

namespace Database
{
    /// <summary>
    /// ORM 表映射，支持基础表和动态分表逻辑
    /// </summary>
    public class OrmMapping
    {
        /// <summary>
        /// 逻辑名称（如 "User"）
        /// </summary>
        public string LogicalName { get; set; }

        /// <summary>
        /// 默认表名（如 "users"）
        /// </summary>
        public string PhysicalTable { get; set; }

        /// <summary>
        /// 分表解析器（动态计算表名的函数，可选）
        /// </summary>
        public Func<object, string> TableResolver { get; set; }

        /// <summary>
        /// 解析最终表名（优先动态分表解析器，未设置则返回默认表名）
        /// </summary>
        public string ResolveTable(object contextData = null)
        {
            return TableResolver != null && contextData != null
                ? TableResolver(contextData)
                : PhysicalTable;
        }
    }
}