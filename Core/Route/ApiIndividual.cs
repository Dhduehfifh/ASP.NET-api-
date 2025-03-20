using System;
using External.Gateway;

namespace Routes
{
    public class ApiIndividual
    {
        private readonly string _path;
        private readonly Requester _requester;  // 🔹【Config 相关】可绑定 `HTTP 请求器`

        public ApiIndividual(string path, Requester requester)
        {
            _path = path;
            _requester = requester;
        }

        public string GetPath()
        {
            return _path;
        }

        public string Execute(string requestData)
        {
            Console.WriteLine($"📡 [ApiIndividual] 处理请求: {_path}，数据: {requestData}");
            return $"✅ 服务器响应: {requestData.ToUpper()}";
        }
    }
}