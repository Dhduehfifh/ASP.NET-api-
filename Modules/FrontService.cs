using External;

namespace Modules.FrontService
{
    public class FrontServiceCore
    {
        public ExternalRequestBase Front_info { get; private set; }
        public Dictionary<string, FigmaPage> Contents { get; set; } = new();

        public static bool IfDebug = false;

        public void AddContent(string name, FigmaPage page)
        {
            if (IfDebug)
            {
                Console.WriteLine($"[Debug] AddContent: {name}");
            }
            Contents[name] = page;
        }

        public void RemoveContent(string name)
        {
            if (IfDebug)
            {
                Console.WriteLine($"[Debug] RemoveContent: {name}");
            }
            if (Contents.ContainsKey(name)) Contents.Remove(name);
        }

        public FigmaPage GetContent(string name)
        {
            if (IfDebug)
            {
                Console.WriteLine($"[Debug] GetContent: {name}");
            }
            return Contents.ContainsKey(name) ? Contents[name] : null;
        }

        public string ExportContentAsJson(string name)
        {
            if (IfDebug)
            {
                Console.WriteLine($"[Debug] ExportContentAsJson: {name}");
            }
            return Contents.ContainsKey(name) ?
                Newtonsoft.Json.JsonConvert.SerializeObject(Contents[name], Newtonsoft.Json.Formatting.Indented) : "{}";
        }

        public void BindExternal(ExternalRequestBase gateway)
        {
            if (IfDebug)
            {
                Console.WriteLine($"[Debug] BindExternal");
            }
            Front_info = gateway;
        }

        public void PushFileToFront(string filePath)
        {
            if (Modules.Api_session.Api_session.IfDebug)
            {
                Console.WriteLine($"[Debug] Push file to front: {filePath}");
            }
            else
            {
                Console.WriteLine($"[Info] File '{filePath}' sent to frontend (simulation).");
            }
        }

        public void Test()
        {
            Console.WriteLine("====== FrontService Test Mode ======");
            foreach (var item in Contents)
            {
                Console.WriteLine($"[Page: {item.Key}]");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(item.Value, Newtonsoft.Json.Formatting.Indented));
            }
            Console.WriteLine("====== End Test ======");
        }
    }

    // --------- Figma 相关定义 --------
    public class FigmaComponent
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public List<string> Class_List { get; set; } = new();
    }

    public class FigmaButton : FigmaComponent
    {
        public string Label { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Action { get; set; } // 标准 string 形式
    }

    public class FigmaForm : FigmaComponent
    {
        public Dictionary<int, FigmaFormLine> Form_content { get; set; } = new();
    }

    public class FigmaFormLine
    {
        public Dictionary<int, FigmaFormCell> Line { get; set; } = new();
    }

    public class FigmaFormCell
    {
        public FigmaComponent Content { get; set; }
    }

    public class FigmaCard : FigmaComponent
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class FigmaPage : FigmaComponent
    {
        public List<FigmaComponent> Children { get; set; } = new();
    }
}