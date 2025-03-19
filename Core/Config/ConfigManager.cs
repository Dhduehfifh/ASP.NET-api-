using System;
using System.Collections.Generic;

namespace Core.Config
{
    public class ConfigManager
    {
        /// <summary>
        /// 🌍 统一管理所有 `Config`
        /// </summary>
        private static readonly Dictionary<Type, IConfig> _configs = new();

        /// <summary>
        /// 🔧 注册 `Config`
        /// </summary>
        public static void UseConfig<T>(T config) where T : IConfig
        {
            _configs[typeof(T)] = config;
        }

        /// <summary>
        /// 📡 获取 `Config`
        /// </summary>
        public static T GetConfig<T>(string apiName, Dictionary<string, object>? overrideValues = null) where T : IConfig, new()
        {
            if (_configs.ContainsKey(typeof(T)))
                return (T)_configs[typeof(T)];

            return (T)(object)CustomConfig.GetOrDefault(apiName, overrideValues);
        }
    }
}