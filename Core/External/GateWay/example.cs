using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Example
{
    public class HttpRequestHandler
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 发送 Jsonfier 数据（支持分区）
        /// </summary>
        public async Task<List<string>> SendJsonfier(string url, Jsonfier jsonfier, int partitionSize = 5)
        {
            var partitions = jsonfier.PartitionData(partitionSize);
            var responses = new List<string>();

            for (int i = 0; i < partitions.Count; i++)
            {
                var partitionedData = partitions[i];
                var jsonString = JsonConvert.SerializeObject(partitionedData);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                using var response = await _httpClient.SendAsync(request);
                string result = await response.Content.ReadAsStringAsync();
                responses.Add(result);
                
                Console.WriteLine($"✅ 发送分区 {i + 1}/{partitions.Count}: {jsonString}");
            }

            return responses;
        }
    }
}