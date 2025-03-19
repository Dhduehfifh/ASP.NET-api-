using System;

public static class DefaultConfig
{
    // 📌 数据库类型（目前仅支持 SQLite，未来可扩展 MySQL/PostgreSQL）
    public static string DatabaseType { get; set; } = "SQLite";

    // 📌 连接字符串（SQLite 默认配置）
    public static string ConnectionString { get; set; } = "Data Source=database.db;";

    // 📌 连接池大小（控制最大连接数）
    public static int PoolSize { get; set; } = 10;

    // 📌 是否启用对象池（默认开启）
    public static bool UsePooling { get; set; } = true;

    /// <summary>
    /// 🌍 `默认配置初始化`（未来可以扩展其他 `Config`）
    /// </summary>
    static DefaultConfig()
    {
        Console.WriteLine("🚀 加载 `DefaultConfig`...");
    }

    /// <summary>
    /// 🛠 手动重置 `Config`（用于调试 & 动态修改）
    /// </summary>
    public static void Reset()
    {
        DatabaseType = "SQLite";
        ConnectionString = "Data Source=database.db;";
        PoolSize = 10;
        UsePooling = true;
        Console.WriteLine("🔄 `DefaultConfig` 已重置！");
    }
}