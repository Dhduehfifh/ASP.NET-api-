using System;
using System.Collections.Generic;

namespace Core.Config
{
    public class CustomConfig : IConfig
    {
        /// <summary>
        /// 🌍 全局默认 `Config`
        /// </summary>
        public static readonly CustomConfig DefaultConfig = new()
        {
            Database = "DefaultDB",
            Timeout = 5000,
            Auth = "None"
        };

        /// <summary>
        /// 📌 `Config` 池化缓存
        /// </summary>
        private static readonly Dictionary<string, CustomConfig> _configCache = new();

        public string Database { get; set; }
        public int Timeout { get; set; }
        public string Auth { get; set; }

        public CustomConfig() { }

        public CustomConfig(CustomConfig other)
        {
            Database = other.Database;
            Timeout = other.Timeout;
            Auth = other.Auth;
        }

        /// <summary>
        /// 🔄 允许 `Config` 继承 `父级 Config`
        /// </summary>
        public void Inject(CustomConfig parent)
        {
            Database ??= parent.Database;
            Timeout = Timeout == 0 ? parent.Timeout : Timeout;
            Auth ??= parent.Auth;
        }

        /// <summary>
        /// 📡 获取 API 级 `Config`，如果没有，则继承 `DefaultConfig`
        /// </summary>
        public static CustomConfig GetOrDefault(string apiName, Dictionary<string, object>? overrideValues = null)
        {
            if (!_configCache.ContainsKey(apiName))
                _configCache[apiName] = new CustomConfig(DefaultConfig);

            var config = _configCache[apiName];

            if (overrideValues != null)
            {
                foreach (var kv in overrideValues)
                {
                    if (kv.Key == "Timeout") config.Timeout = Convert.ToInt32(kv.Value);
                    if (kv.Key == "Database") config.Database = kv.Value.ToString();
                    if (kv.Key == "Auth") config.Auth = kv.Value.ToString();
                }
            }

            return config;
        }
    }
}