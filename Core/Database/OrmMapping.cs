using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Database
{
    public class OrmMapping
    {
        public string LogicalName { get; set; }
        public string PhysicalTableName { get; set; }

        // 预留扩展：未来按条件动态分表
        public string ResolveTable(object contextData = null)
        {
            return PhysicalTableName;
        }
    }
}