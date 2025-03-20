using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace External.Gateway
{
    public class Requester
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private string? _baseUrl; // ❌ 默认不设值，强制用户赋值

        /// <summary>
        /// 🌍 设定默认 API URL（支持 `.xxx.xxx` 方式赋值）
        /// </summary>
        public Requester WithBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }

        /// <summary>
        /// 🌍 发送请求（如果 URL 未设置，则报错）
        /// </summary>
        public async Task<string> Fetch(string endpoint, string jsonPayload)
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                throw new InvalidOperationException($"❌ [Requester] 未设置 `BaseUrl`，请调用 `WithBaseUrl()` 进行赋值！");
            }

            string url = $"{_baseUrl}{endpoint}";
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
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