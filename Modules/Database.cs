using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Modules.Database
{
    public class DatabaseCore
    {
        private readonly string _connectionString;
        public Dictionary<string, List<DatabaseItem>> Contents { get; set; } = new(); // 存储所有表和数据
        public static bool IfDebug = false; // 全局调试开关

        public DatabaseCore(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString), "ConnectionString cannot be null or empty");
            _connectionString = connectionString;
        }

        // 添加数据到指定表
        public void AddData(string table, DatabaseItem item)
        {
            if (IfDebug)
            {
                Console.WriteLine($"[Debug] AddData to Table: {table} => {item.ToRawString()}");
                return;
            }

            if (!Contents.ContainsKey(table)) Contents[table] = new List<DatabaseItem>();
            Contents[table].Add(item);

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = $"INSERT INTO {table} (Data) VALUES (@Data)";
            cmd.Parameters.AddWithValue("@Data", item.ToRawString());
            cmd.ExecuteNonQuery();
        }

        // 查询表数据
        public List<DatabaseItem> GetData(string table)
        {
            if (IfDebug)
            {
                Console.WriteLine($"[Debug] GetData from Table: {table}");
                return Contents.ContainsKey(table) ? Contents[table] : new List<DatabaseItem>();
            }

            var results = new List<DatabaseItem>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT Data FROM {table}";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var data = reader.GetString(0);
                results.Add(new DatabaseItem(data));
            }

            return results;
        }

        // 删除表中所有数据
        public void ClearTable(string table)
        {
            if (IfDebug)
            {
                Console.WriteLine($"[Debug] ClearTable: {table}");
                return;
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = $"DELETE FROM {table}";
            cmd.ExecuteNonQuery();

            if (Contents.ContainsKey(table)) Contents[table].Clear();
        }

        // 测试函数输出所有表数据
        public void Test()
        {
            Console.WriteLine("====== Database Test Mode ======");
            foreach (var table in Contents)
            {
                Console.WriteLine($"[Table: {table.Key}]");
                foreach (var item in table.Value)
                {
                    Console.WriteLine(item.ToRawString());
                }
            }
            Console.WriteLine("====== End Test ======");
        }
    }

    // 存储单条数据的类，保持简洁，标准格式
    public class DatabaseItem
    {
        public string RawData { get; set; }

        public DatabaseItem(string raw)
        {
            RawData = raw ?? throw new ArgumentNullException(nameof(raw), "Data cannot be null");
        }

        public string ToRawString() => RawData;
    }
}