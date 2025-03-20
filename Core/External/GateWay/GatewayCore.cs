using System;

namespace External.Gateway
{
    public class GatewayCore
    {
        private string? _baseUrl; // ❌ 默认不设值，强制用户赋值

        /// <summary>
        /// 🌍 设定 API 基础 URL（支持 `.xxx.xxx` 方式赋值）
        /// </summary>
        public GatewayCore WithBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }

        /// <summary>
        /// 🌍 获取当前 URL（如果未设置 URL，则报错）
        /// </summary>
        public string GetBaseUrl()
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                throw new InvalidOperationException($"❌ [GatewayCore] 未设置 `BaseUrl`，请调用 `WithBaseUrl()` 进行赋值！");
            }
            return _baseUrl;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"✅ Gateway URL: {GetBaseUrl()}");
        }

        /// <summary>
        /// 🌍 释放资源（归还对象池）
        /// </summary>
        public void Reset()
        {
            _baseUrl = null;
        }
    }
}