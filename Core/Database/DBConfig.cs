

namespace Database
{
    public class DBConfig
    {
        public string DatabaseType { get; set; }
        public string ConnectionString { get; set; }
        
        // 给后面留着orm映射
        public Dictionary<string, string> TableMappings { get; set; }

        public string ParamStyle { get; set; } = "@";
        public bool Transactional { get; set; } = true;

        //获取适配器（暂时为空，后期再尝试实现）
        public ISqlAdapter GetSqlAdapter()
        {
            //按照实际情况返回适配器，暂时留空
            throw new NotImplementedException();
        }
    }
}