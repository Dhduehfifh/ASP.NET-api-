using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using Microsoft.Extensions.ObjectPool;

namespace External.Gateway
{
    public class Requester
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public bool If_assign { get; set; } = false;
        public Jsonfier Req { get; private set; } = new Jsonfier();
        public Jsonfier Res { get; private set; } = new Jsonfier();
        private readonly Queue<string> _requestQueue = new(); // 存储 JSON 字符串请求

        // 请求类型枚举
        public enum Request_Type
        {
            GET,
            POST,
            PUT,
            PATCH,
            DELETE,
            HEAD,
            TRACE,
            CONNECT
        }

        private static readonly Dictionary<Request_Type, HttpMethod> HttpMethodsMap = new()
        {
            { Request_Type.GET, HttpMethod.Get },
            { Request_Type.POST, HttpMethod.Post },
            { Request_Type.PUT, HttpMethod.Put },
            { Request_Type.PATCH, HttpMethod.Patch },
            { Request_Type.DELETE, HttpMethod.Delete },
            { Request_Type.HEAD, HttpMethod.Head },
            { Request_Type.TRACE, HttpMethod.Trace },
            { Request_Type.CONNECT, HttpMethod.Options }
        };

        /// <summary>
        /// 发送 `Jsonfier` 数据（完整或增量）
        /// </summary>
        public async Task Fetch(Jsonfier req, string url, Request_Type type, bool sendDiff = false)
        {
            string jsonString = sendDiff ? req.SerializeDiff() : req.SerializeFull();
            _requestQueue.Enqueue(jsonString); // 存入队列

            while (_requestQueue.Count > 0)
            {
                var requestData = _requestQueue.Dequeue();
                var content = new StringContent(requestData, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethodsMap[type], url)
                {
                    Content = content
                };

                using var response = await _httpClient.SendAsync(request);
                string responseString = await response.Content.ReadAsStringAsync();

                // 解析服务器返回的数据，并更新 `Jsonfier`
                Res.Deserialize(responseString);
            }
        }

        /// <summary>
        /// 获取完整返回数据
        /// </summary>
        public Dictionary<string, object> GetResponseData()
        {
            return Res.GetFull();
        }

        /// <summary>
        /// 获取服务器返回的增量数据
        /// </summary>
        public Dictionary<string, object> GetResponseDiff()
        {
            return Res.GetDiff();
        }

        /// <summary>
        /// 清空对象，避免数据污染（用于对象池）
        /// </summary>
        public void Reset()
        {
            If_assign = false;
            Req = new Jsonfier();
            Res = new Jsonfier();
            _requestQueue.Clear();
        }
    }

    // 🔹 `Requester` 对象池
    public static class RequesterPool
    {
        private static readonly ObjectPool<Requester> _pool = new DefaultObjectPool<Requester>(new RequesterPolicy(), 100);

        public static Requester Rent() => _pool.Get();
        public static void Return(Requester requester) => _pool.Return(requester);
    }

    // 🔹 `Requester` 复用策略
    public class RequesterPolicy : PooledObjectPolicy<Requester>
    {
        public override Requester Create() => new Requester();

        public override bool Return(Requester requester)
        {
            requester.Reset();
            return true;
        }
    }
}