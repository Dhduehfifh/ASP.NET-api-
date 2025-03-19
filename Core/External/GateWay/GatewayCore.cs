using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toolbox;

namespace External.Gateway
{
    public class GatewayCore
    {
        private readonly Requester _requester;
        private readonly string _baseUrl;

        public GatewayCore(string baseUrl)
        {
            _requester = new Requester();
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// 🌍 发送 JSON 请求
        /// </summary>
        public async Task<Jsonfier> SendRequest(string endpoint, Jsonfier payload, Requester.Request_Type method)
        {
            string fullUrl = $"{_baseUrl}{endpoint}";
            await _requester.Fetch(payload, fullUrl, method);
            return _requester.Res; // 返回响应
        }
    }
}