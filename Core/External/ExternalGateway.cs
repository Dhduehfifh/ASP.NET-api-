using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace External
{
    public class ExternalGateway
    {
        private readonly HttpClient _httpClient;

        public ExternalGateway()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 发送 Jsonfier 数据到外部 API（POST 请求）
        /// </summary>
        public async Task<string> SendJsonData(string url, Jsonfier jsonData)
        {
            var jsonString = JsonConvert.SerializeObject(jsonData.Fields);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 发送文件（支持 URL+CDN 或者 File Stream）
        /// </summary>
        public async Task<string> SendFile(string url, string filePath, bool useCdn = false)
        {
            if (useCdn)
            {
                // CDN 模式，直接返回 URL
                return $"https://cdn.example.com/{filePath}";
            }

            // File Stream 模式
            using var content = new MultipartFormDataContent();
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 发送 GET 请求到外部 API（用于查询或状态获取）
        /// </summary>
        public async Task<string> FetchData(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }
}