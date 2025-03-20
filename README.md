

# 🚀  API Framework

**是一个轻量级但企业级可扩展的 ASP.NET API 框架，支持 `API 组件化绑定`、`DSL 风格调用`、`前端适配` 和 `外部网关集成`。**


## 🔹 **快速开始**
### **1️⃣ 克隆仓库**
```sh
git clone https://github.com/Dhduehfifh/ASP.NET-api-.git
cd ASP.NET-api-
```
2️⃣ 构建并运行
```
dotnet build
dotnet run
```
3️⃣ 绑定 API
```
📌 示例代码（Program.cs）

var apiRoutes = new ApiSet("/API")
{
    .BindApi(new ApiIndividual("/GetData", new Requester()))
    .BindApi(new ApiIndividual("/PostData", new Requester()))
    .BindDb(new DatabaseCore()
        .WithConfig(new DBConfig()
        {
            ConnectionString = "Data Source=database.db;Version=3;",
            Provider = "SQLite"
        }))
    .BindGateway(new GatewayCore()
        .WithBaseUrl("https://api.external-service.com"))
};
```


⸻

🔹 API 组件

🟢 API 集合（ApiSet.cs）
	•	负责 管理 API 逻辑
	•	可绑定 API 个体、数据库、外部 API 网关
	•	支持 DSL 绑定
```
public class ApiSet
{
    public ApiSet BindApi(ApiIndividual api) { }
    public ApiSet BindDb(DatabaseCore database) { }
    public ApiSet BindGateway(GatewayCore gateway) { }
}
```
🟢 API 个体（ApiIndividual.cs）
	•	负责 单个 API 的请求/响应
	•	可被 ApiSet 绑定
```
public class ApiIndividual
{
    public string GetPath() { }
    public string Execute(string requestData) { }
}
```


⸻

🔹 数据库管理

📌 配置 数据库
```
var db = new DatabaseCore()
    .WithConfig(new DBConfig()
    {
        ConnectionString = "Data Source=database.db;Version=3;",
        Provider = "SQLite"
    });

```

⸻

🔹 前端适配

📌 绑定 前端 UI 路由
```
var staticRoutes = new StaticRoute()
    .BindUI("/ui/vue", "vue")
    .BindStatic("/static/vue-app.js", "vue-app");
```
📌 前端 HTML 页面
```
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Vue UI</title>
    <script src="/static/vue-app.js"></script>
</head>
<body>
    <div id="app"></div>
</body>
</html>

```

⸻

🔹 文件路由

📌 绑定 文件 API
```
var fileRoutes = new FileRoute()
    .BindFile("/files/example.pdf", "example.pdf");
```
📌 访问 文件 API
```
GET /files/example.pdf
```


⸻

🔹 外部 API 网关

📌 配置 API Gateway
```
var gateway = new GatewayCore()
    .WithBaseUrl("https://api.external-service.com");
```
📌 Requester 处理 API 请求
```
var requester = new Requester()
    .WithBaseUrl("https://api.external-service.com");

```



📌 许可证

本项目基于 MIT 许可证，允许自由使用、修改和分发。

