# ASP.NET API Gateway Framework

一个高度模块化、可插件化、支持多数据源和分布式 AI 调用的 ASP.NET Core API 框架。支持 DLL 插件、对象池、负载监测、数据库池化、差分存储、模板共享机制等高级特性。

---

## ✅ 返回对象调用示例（只列代码）

```csharp
// JSON 返回对象
return new {
    status = "ok",
    message = "操作成功",
    data = new { id = 1, name = "test" }
};
```

```csharp
// HTML 返回对象
return new HtmlResult("templateName", new {
    title = "页面标题",
    content = "这是内容"
});
```

```csharp
// 文件返回对象
return new FileResult("files/report.pdf", "application/pdf");
```

```csharp
// 自定义内容类型返回对象
return new CustomResult("<xml><ok>true</ok></xml>", "application/xml");
```

```csharp
// 插件调用对象
var result = PluginManager.Invoke("MyPlugin", new {
    input = "参数值",
    token = "安全令牌"
});
return new {
    status = "plugin_result",
    output = result
};
```

```csharp
// AI Agent 调用对象（预设接口）
var result = AiAgent.Call("AnalyzeText", new {
    text = "用户输入内容"
});
return new {
    ai_response = result
};
```

---

## ✅ 外部网关 / 数据库 / 前端返回结构调用示例合集

```csharp
// 调用外部网关（ExternalGateway）示例
var response = ExternalGateway.Call("WeatherService/GetNow", new {
    city = "Toronto",
    lang = "en"
});
return new {
    gateway = "external",
    result = response
};
```

```csharp
// 调用数据库（Database）模块示例
var users = Database.Query("Users", where: new {
    role = "admin"
});
return new {
    from = "database",
    list = users
};
```

```csharp
// 前端返回消息（适配 Vue / JSON）示例
return new {
    frontend = true,
    notify = new {
        type = "info",
        content = "操作已完成"
    }
};
```

---

## ✅ Program.cs 启动 + 注册 API 示例（可直接运行）

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 创建并注册用户 API 集合
ApiRouteSet usersApi = new("/api/users");

usersApi.Add("/get", args => {
    var result = Database.Query("Users", where: new {
        active = true
    });
    return new {
        data = result
    };
});

usersApi.Add("/post", args => {
    var insertResult = Database.Insert("Users", new {
        name = args["name"],
        email = args["email"]
    });
    return new {
        status = "inserted",
        result = insertResult
    };
});

usersApi.Add("/notify", args => {
    return new {
        frontend = true,
        notify = new {
            type = "success",
            content = "通知已发送"
        }
    };
});

usersApi.Add("/external", args => {
    var data = ExternalGateway.Call("NewsService/Top", new {
        category = "tech"
    });
    return new {
        source = "external",
        articles = data
    };
});

RouteRegistry.Register(usersApi);

// 启动服务器并监听
app.Map("/api/{**path}", ApiDispatcher.Handle);
app.Run();
```

---

（本段为可直接运行的最简 Program.cs 示例，支持用户 API 注册、前端消息、数据库交互与外部网关调用）

