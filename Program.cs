using Modules.Api_session;
using Modules.Database;
using Modules.FrontService;
using External;
using System;

class Program
{
    static void Main(string[] args)
    {
        // ---------------- 初始化 API 集合 ----------------
        var api = new Api_session();

        // ---------------- 开启 DEBUG 模式 ----------------
        Api_session.IfDebug = true; // 注意：一键全局 Debug

        // ---------------- 绑定主外部网关 ----------------
        var mainGateway = new ExternalGateway("https://api.mainserver.com/getdata");
        api.BindGateway(mainGateway);

        // ---------------- 绑定备用外部网关 ----------------
        api.AddExternal(new ExternalGateway("https://api.backupserver1.com/data"));
        api.AddExternal(new ExternalGateway("https://api.backupserver2.com/data"));

        // ---------------- 绑定数据库 ----------------
        var db = new DatabaseCore("Server=localhost;Database=TestDB;Trusted_Connection=True;");
        api.BindDatabase(db);

        // ---------------- 绑定前端服务 ----------------
        var front = new FrontServiceCore();
        api.BindFront(front);

        // ---------------- 注册页面组件（Figma 模拟页面） ----------------
        var homePage = new FigmaPage
        {
            Id = "homePage",
            Type = "page",
            Children = new List<FigmaComponent>
            {
                new FigmaButton { Id = "btnWelcome", Type = "button", Label = "Welcome", Color = "blue", Size = "large", Action = "gotoDetail()" },
                new FigmaCard { Id = "cardIntro", Type = "card", Title = "Intro Card", Content = "Welcome to our AI system!" }
            }
        };
        front.Contents["home"] = homePage;

        // ---------------- 路由绑定 ----------------

        // 首页路由
        api.BindRouter("home", new Api_individual(api, new RequestJsonContainer("{ \"prompt\": \"Get homepage info\", \"source\": \"azure\" }"), "home.html"));

        // 详情页路由
        api.BindRouter("detail", new Api_individual(api, new RequestJsonContainer("{ \"prompt\": \"Get detail info\", \"source\": \"plugin\" }"), "detail.html"));

        // AI 测试路由
        api.BindRouter("ai", new Api_individual(api, new RequestJsonContainer("{ \"prompt\": \"AI, what is today's news?\", \"source\": \"colab\" }"), "ai.html"));

        // ---------------- 执行路由测试 ----------------
        Console.WriteLine("\n--- Route System Ready ---\n");

        api.CallRoute("home");    // 执行 home 路由
        api.CallRoute("detail");  // 执行 detail 路由
        api.CallRoute("ai");      // 执行 AI 测试路由

        // ---------------- API 集合测试 ----------------
        Console.WriteLine("\n--- Api_session Self Test ---\n");
        api.Test();

        // ---------------- 完成 ----------------
        Console.WriteLine("\n--- System Initialized ---\n");
        Console.ReadKey(); // 防止窗口闪退
    }
}