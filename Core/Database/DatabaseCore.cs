using System;
using System.Data.SQLite;

namespace Core.Database
{
    public class DatabaseCore
    {
        private SQLiteConnection? _connection;

        public DatabaseCore()
        {
            if (DefaultConfig.DatabaseType == "SQLite")
            {
                _connection = new SQLiteConnection(DefaultConfig.ConnectionString);
            }
        }

        public void Connect()
        {
            _connection?.Open();
            Console.WriteLine($"🚀 连接 `SQLite` 数据库: {DefaultConfig.ConnectionString}");
        }

        public void Close()
        {
            _connection?.Close();
            Console.WriteLine("🔌 关闭 `SQLite` 连接");
        }

        public SQLiteConnection? GetConnection()
        {
            return _connection;
        }
    }
}