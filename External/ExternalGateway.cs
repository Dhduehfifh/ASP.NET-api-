using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace External
{
    // Global gateway (实例类)
    public class ExternalGateway
    {
        private readonly string _url;
        private RequestJsonContainer _jsonContainer;

        public static bool IfDebug = true; // 全局调试开关

        // 实例构造，接收 URL
        public ExternalGateway(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url), "URL cannot be null or empty");
            _url = url;
        }

        // 实例 Test 方法
        public void Test()
        {
            try
            {
                Console.WriteLine($"[Info] Fetching JSON from: {_url}");

                using var client = new HttpClient();
                var jsonString = client.GetStringAsync(_url).Result;

                Console.WriteLine("[Fetch Raw JSON] " + jsonString);

                _jsonContainer = new RequestJsonContainer(jsonString);

                var request = RequestFactory.CreateRequest(_jsonContainer);

                request.Execute();

                Console.WriteLine("[Standardized Result] " + request.GetStandardResponse());
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error] " + ex.Message);
            }
        }

        // 在 ExternalGateway.cs 内添加
public string GetUrl()
{
    return _url;
}

public void Fetch(string json)
{
    if (IfDebug)
    {
        Console.WriteLine($"[Debug] Fetching with JSON: {json} from {_url}");
    }

    // 实际 fetch 略去，保留测试逻辑
}
    }

   

    public class RequestJsonContainer
    {
        public JObject JsonObject { get; private set; }
        public string JsonRaw { get; private set; }

        public RequestJsonContainer(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json), "Input JSON cannot be null or empty");
            JsonRaw = json;
            JsonObject = JObject.Parse(json);
            if (ExternalGateway.IfDebug)
                Console.WriteLine("[Debug] JSON Loaded: " + JsonObject.ToString());
        }

        public JToken GetToken(params string[] path)
        {
            if (path == null || path.Length == 0) throw new ArgumentNullException(nameof(path), "Path cannot be null or empty");
            JToken token = JsonObject;
            foreach (var p in path)
            {
                if (token[p] == null) throw new Exception($"Path '{string.Join(".", path)}' not found");
                token = token[p];
            }
            return token;
        }

        public string ToRawString() => JsonObject.ToString();
    }

    public class ResponseStandardizer
    {
        public JObject StandardResponse { get; private set; }

        public ResponseStandardizer(JObject rawResponse)
        {
            if (rawResponse == null) throw new ArgumentNullException(nameof(rawResponse), "Response cannot be null");
            StandardResponse = new JObject
            {
                ["result"] = rawResponse.ToString(),
                ["extra"] = new JObject
                {
                    ["source"] = "undefined",
                    ["timestamp"] = DateTime.UtcNow.ToString("o")
                }
            };
            if (ExternalGateway.IfDebug)
                Console.WriteLine("[Debug] Response Standardized");
        }

        public string ToStandardResult() => StandardResponse.ToString();

        public void SetSource(string source)
        {
            if (string.IsNullOrWhiteSpace(source)) throw new ArgumentNullException(nameof(source), "Source cannot be null or empty");
            StandardResponse["extra"]["source"] = source;
        }

        public void AddExtra(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), "Key cannot be null or empty");
            StandardResponse["extra"][key] = JToken.FromObject(value);
        }
    }

    public abstract class ExternalRequestBase
    {
        public RequestJsonContainer RequestData { get; private set; }
        public ResponseStandardizer ResponseData { get; protected set; }

        protected ExternalRequestBase(RequestJsonContainer requestData)
        {
            RequestData = requestData ?? throw new ArgumentNullException(nameof(requestData), "Request data cannot be null");
            ResponseData = new ResponseStandardizer(new JObject());
        }

        public abstract void Execute();

        public string GetStandardResponse() => ResponseData.ToStandardResult();
    }

    public class AzureRequest : ExternalRequestBase
    {
        public AzureRequest(RequestJsonContainer requestData) : base(requestData) { }

        public override void Execute()
        {
            var simulatedResponse = new JObject
            {
                ["answer"] = $"Hello, this is Azure AI response to: {RequestData.GetToken("prompt")}"
            };
            ResponseData = new ResponseStandardizer(simulatedResponse);
            ResponseData.SetSource("azure");
            ResponseData.AddExtra("model", "gpt-4");
        }
    }

    public class AwsRequest : ExternalRequestBase
    {
        public AwsRequest(RequestJsonContainer requestData) : base(requestData) { }

        public override void Execute()
        {
            var placeholder = new JObject
            {
                ["answer"] = $"AWS placeholder response to: {RequestData.GetToken("prompt")}"
            };
            ResponseData = new ResponseStandardizer(placeholder);
            ResponseData.SetSource("aws");
        }
    }

    public class ColabRequest : ExternalRequestBase
    {
        public ColabRequest(RequestJsonContainer requestData) : base(requestData) { }

        public override void Execute()
        {
            var placeholder = new JObject
            {
                ["answer"] = $"Colab placeholder response to: {RequestData.GetToken("prompt")}"
            };
            ResponseData = new ResponseStandardizer(placeholder);
            ResponseData.SetSource("colab");
        }
    }

    public class PluginRequest : ExternalRequestBase
    {
        public PluginRequest(RequestJsonContainer requestData) : base(requestData) { }

        public override void Execute()
        {
            var placeholder = new JObject
            {
                ["answer"] = $"Plugin placeholder response to: {RequestData.GetToken("prompt")}"
            };
            ResponseData = new ResponseStandardizer(placeholder);
            ResponseData.SetSource("plugin");
        }
    }

    public class RequestFactory
    {
        public static ExternalRequestBase CreateRequest(RequestJsonContainer jsonContainer)
        {
            if (jsonContainer == null) throw new ArgumentNullException(nameof(jsonContainer), "JsonContainer cannot be null");

            var source = jsonContainer.GetToken("source")?.ToString()?.ToLower() ?? "azure";

            return source switch
            {
                "aws" => new AwsRequest(jsonContainer),
                "azure" => new AzureRequest(jsonContainer),
                "colab" => new ColabRequest(jsonContainer),
                "plugin" => new PluginRequest(jsonContainer),
                _ => throw new NotSupportedException($"Unknown source type '{source}'")
            };
        }
    }
}