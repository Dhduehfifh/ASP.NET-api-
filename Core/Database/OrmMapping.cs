using System;
using System.Collections.Generic;

namespace Core.Database
{
    public class OrmMapping
    {
        private static readonly Dictionary<Type, string> _tableMappings = new();

        public static void Register<T>(string tableName)
        {
            _tableMappings[typeof(T)] = tableName;
        }

        public static string GetTableName<T>()
        {
            return _tableMappings.TryGetValue(typeof(T), out var tableName) ? tableName : typeof(T).Name;
        }
    }
}