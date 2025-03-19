using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Core.Database
{
    public class DbContext
    {
        private readonly DatabaseCore _database;

        public DbContext()
        {
            _database = new DatabaseCore();
            _database.Connect();
        }

        public void Dispose()
        {
            _database.Close();
        }

        public List<T> Query<T>(string sql, Func<IDataReader, T> map)
        {
            var resultList = new List<T>();
            var connection = _database.GetConnection();
            if (connection == null) return resultList;

            using (var command = new SQLiteCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    resultList.Add(map(reader));
                }
            }

            return resultList;
        }

        public int Execute(string sql)
        {
            var connection = _database.GetConnection();
            if (connection == null) return -1;

            using (var command = new SQLiteCommand(sql, connection))
            {
                return command.ExecuteNonQuery();
            }
        }
    }
}