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
        private readonly Queue<ProtocolChunk> _requestQueue = new();

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
        /// 发送 `Jsonfier` 数据，自动分块传输
        /// </summary>
        public async Task Fetch(Jsonfier req, string url, Request_Type type, int chunkSize = 1024)
        {
            var chunks = req.Chunk(req, chunkSize);
            foreach (var chunk in chunks)
            {
                _requestQueue.Enqueue(chunk); // 逐块加入队列
            }

            while (_requestQueue.Count > 0)
            {
                var chunkData = _requestQueue.Dequeue();
                var jsonString = req.ChunkToJson(chunkData);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethodsMap[type], url)
                {
                    Content = content
                };

                using var response = await _httpClient.SendAsync(request);
                string responseString = await response.Content.ReadAsStringAsync();

                // 添加到 `Jsonfier` 作为接收数据
                var receivedChunk = req.ChunkFromJson(responseString);
                Res.AddChunk(receivedChunk);
            }
        }

        /// <summary>
        /// 获取完整数据（如果数据传输完毕）
        /// </summary>
        public byte[] GetCompleteResponse()
        {
            return Res.IsComplete ? Res.Reassemble() : null;
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