using System;
using System.Collections.Generic;
using External;
using Newtonsoft.Json.Linq;
using Modules.Database;
using Modules.FrontService;

namespace Modules.Api_session
{
    public class Api_session
    {
        public static bool IfDebug = false;

        // ------------------- 网关绑定区 -------------------
        public List<ExternalGateway> ExternalGateways { get; private set; } = new();
        public ExternalGateway MainGateway { get; private set; }

        // ------------------- API 路由 -------------------
        public Dictionary<string, Api_individual> ApiRoutes { get; private set; } = new();

        public DatabaseCore Database { get; private set; } // 使用 DatabaseCore，避免命名冲突
        public FrontServiceCore Front { get; private set; } // FrontServiceCore 同理

        // 补充：
        public Api_session BindDatabase(DatabaseCore db)
        {
            Database = db ?? throw new ArgumentNullException(nameof(db));
            return this;
        }

        public Api_session BindFront(FrontServiceCore front)
        {
            Front = front ?? throw new ArgumentNullException(nameof(front));
            return this;
        }

        // ------------------- 绑定方法 -------------------
        public Api_session BindGateway(ExternalGateway gateway)
        {
            MainGateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            return this;
        }

        public Api_session AddExternal(ExternalGateway gateway)
        {
            ExternalGateways.Add(gateway ?? throw new ArgumentNullException(nameof(gateway)));
            return this;
        }

        public Api_session BindRouter(string name, Api_individual api)
        {
            ApiRoutes[name] = api;
            return this;
        }

        // ------------------- 路由调用 -------------------
        public void CallRoute(string name)
        {
            if (ApiRoutes.ContainsKey(name))
            {
                if (IfDebug) Console.WriteLine($"[Debug] API Called: {name}");
                ApiRoutes[name].Execute();
            }
            else Console.WriteLine($"[Error] API '{name}' not found");
        }

        // ------------------- 测试 -------------------
        public void Test()
        {
            IfDebug = true;
            Console.WriteLine("----- Running Api_session Test (Personal Test) -----");

            var reqJson = new RequestJsonContainer("{ \"prompt\": \"Test Debug\", \"source\": \"azure\" }");
            var api = new Api_individual(this, reqJson, "test.html");
            api.Execute();

            Console.WriteLine(api.GetResponse());

            Console.WriteLine("----- Test End -----");
        }
    }

    // ------------------- API 路由个体 -------------------
    public class Api_individual
    {
        public Api_session Father { get; private set; }
        public RequestJsonContainer RequestData { get; private set; }
        public string FrontFile { get; private set; }
        private ResponseStandardizer Response { get; set; }

        public Api_individual(Api_session session, RequestJsonContainer request, string frontFile = null)
        {
            Father = session ?? throw new ArgumentNullException(nameof(session));
            RequestData = request ?? throw new ArgumentNullException(nameof(request));
            FrontFile = frontFile;
        }

        public void Execute()
        {
            if (Api_session.IfDebug)
            {
                Console.WriteLine($"[Debug] Request: {RequestData.ToRawString()}");
            }

            // ------------------- 网关执行 -------------------
            if (Father.MainGateway != null)
            {
                if (Api_session.IfDebug)
                {
                    Console.WriteLine($"[Debug] Main Gateway URL: {Father.MainGateway.GetUrl()}");
                }

                Father.MainGateway.Fetch(RequestData.ToRawString());

                Response = new ResponseStandardizer(new JObject
                {
                    ["result"] = $"Fetched from Main Gateway: {Father.MainGateway.GetUrl()}",
                    ["source"] = "gateway"
                });
            }

            // ------------------- 外部网关依次调用 -------------------
            foreach (var gateway in Father.ExternalGateways)
            {
                if (Api_session.IfDebug)
                {
                    Console.WriteLine($"[Debug] External Gateway URL: {gateway.GetUrl()}");
                }

                gateway.Fetch(RequestData.ToRawString());
            }

            // ------------------- 前端推送 -------------------
            Father.Front?.PushFileToFront(FrontFile);

            // ------------------- 数据库存储 -------------------
            // ------------------- 数据库 -------------------
// 正确的数据库写入逻辑
            if (Father.Database is DatabaseCore dbCore)
            {
                dbCore.AddData("api_log", new DatabaseItem(
                    $"{{ \"Request\": {RequestData.ToRawString()}, \"Response\": {Response?.ToStandardResult() ?? "\"No Response\""} }}"
                ));

                if (Api_session.IfDebug)
                {
                    Console.WriteLine($"[Debug] Database Added: Request & Response logged to 'api_log'");
                }
            }

            // ------------------- 默认返回 -------------------
            if (Response == null)
            {
                Response = new ResponseStandardizer(new JObject
                {
                    ["result"] = "No gateway available for request",
                    ["source"] = "system"
                });
            }
        }

        public string GetResponse()
        {
            return Response?.ToStandardResult() ?? "{ \"error\": \"No response generated\" }";
        }
    }
}