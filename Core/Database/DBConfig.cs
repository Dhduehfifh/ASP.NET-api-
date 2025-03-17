namespace Database
{
    public class DBConfig
    {
        // 适配器（必须手动传入，避免硬编码）
        public ISqlAdapter SqlAdapter { get; set; }

        // 默认参数（可调整）
        public int PoolSize { get; set; } = 200;  // 默认连接池大小
        public int Timeout { get; set; } = 30;    // 默认超时 30 秒
        public bool AutoReconnect { get; set; } = true;  // 默认自动重连
        public string Region { get; set; } = "local";  // 服务器区域

        // 额外自定义字段
        public Dictionary<string, object> Extensions { get; set; } = new();

        // 默认构造
        public DBConfig() { }

        // 允许直接传入适配器
        public DBConfig(ISqlAdapter adapter)
        {
            SqlAdapter = adapter;
        }

        // 统一适配器获取
        public ISqlAdapter GetSqlAdapter() => SqlAdapter;
    }

    public interface ISqlAdapter
    {
        void Execute(string sql, Dictionary<string, object> parameters);
    }
}