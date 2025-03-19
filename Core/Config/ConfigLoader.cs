using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Core.Config
{
    public class ConfigLoader
    {
        /// <summary>
        /// 📡 读取 `config.json`
        /// </summary>
        public static Dictionary<string, object> LoadFromJson(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"❌ 配置文件 `{path}` 未找到");

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
    }
}