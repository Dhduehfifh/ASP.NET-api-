using System;
using System.Collections.Generic;

namespace Core.Config
{
    public class ConfigValidator
    {
        public static void Validate(string apiName, Dictionary<string, object> newConfig)
        {
            var existingConfig = CustomConfig.GetOrDefault(apiName);

            foreach (var key in newConfig.Keys)
            {
                var existingValue = existingConfig.GetType().GetProperty(key)?.GetValue(existingConfig);
                var newValue = newConfig[key];

                if (!Equals(existingValue, newValue))
                {
                    throw new Exception($"❌ 配置冲突: API `{apiName}` 的 `{key}` 值 `{existingValue}` 与新配置 `{newValue}` 不匹配");
                }
            }
        }
    }
}