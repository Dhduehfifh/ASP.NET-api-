using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.ObjectPool;

namespace Toolbox
{
    /// <summary>
    /// Json 数据管理工具，支持增量更新（diff）、对象池化（减少 GC）、KV 嵌套 JSON
    /// </summary>
    public class Jsonfier
    {
        private Dictionary<string, object> _data = new();   // 主数据存储
        private Dictionary<string, object> _diff = new();   // 仅存增量数据
        private DateTime _lastUpdate = DateTime.UtcNow;     // 记录最后更新时间

        /// <summary>
        /// 设置数据（完整更新，同时存入 `diff`）
        /// </summary>
        public void Set(string key, object value)
        {
            _data[key] = value;
            _diff[key] = value; // 记录到增量更新
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        public object Get(string key)
        {
            return _data.ContainsKey(key) ? _data[key] : null;
        }

        /// <summary>
        /// 仅获取增量更新数据
        /// </summary>
        public Dictionary<string, object> GetDiff()
        {
            return _diff;
        }

        /// <summary>
        /// 获取完整数据
        /// </summary>
        public Dictionary<string, object> GetFull()
        {
            return _data;
        }

        /// <summary>
        /// 序列化完整数据（包含元数据）
        /// </summary>
        public string SerializeFull()
        {
            var json = JsonSerializer.Serialize(new { meta = new { version = "1.0", last_update = _lastUpdate }, data = _data },
                new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true });

            _diff.Clear(); // 发送后清空 diff
            return json;
        }

        /// <summary>
        /// 序列化增量数据
        /// </summary>
        public string SerializeDiff()
        {
            var json = JsonSerializer.Serialize(new { diff = _diff },
                new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true });

            _diff.Clear(); // 发送后清空 diff
            return json;
        }

        /// <summary>
        /// 反序列化完整数据
        /// </summary>
        public void Deserialize(string json)
        {
            var parsed = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            if (parsed.ContainsKey("data"))
            {
                _data = JsonSerializer.Deserialize<Dictionary<string, object>>(parsed["data"].ToString());
            }
            if (parsed.ContainsKey("diff"))
            {
                _diff = JsonSerializer.Deserialize<Dictionary<string, object>>(parsed["diff"].ToString());
            }
        }

        /// <summary>
        /// 仅应用 `diff` 更新数据
        /// </summary>
        public void ApplyDiff(string diffJson)
        {
            var parsedDiff = JsonSerializer.Deserialize<Dictionary<string, object>>(diffJson);
            if (parsedDiff.ContainsKey("diff"))
            {
                var diffData = JsonSerializer.Deserialize<Dictionary<string, object>>(parsedDiff["diff"].ToString());
                foreach (var key in diffData.Keys)
                {
                    _data[key] = diffData[key];
                }
            }
        }

        /// <summary>
        /// 清空数据（用于池化复用）
        /// </summary>
        public void Reset()
        {
            _data.Clear();
            _diff.Clear();
        }
    }

    /// <summary>
    /// Jsonfier 对象池（减少 new，优化性能）
    /// </summary>
    public static class JsonfierPool
    {
        private static readonly ObjectPool<Jsonfier> _pool = new DefaultObjectPool<Jsonfier>(new JsonfierPolicy(), 100);

        public static Jsonfier Rent() => _pool.Get();
        public static void Return(Jsonfier jsonfier) => _pool.Return(jsonfier);
    }

    public class JsonfierPolicy : PooledObjectPolicy<Jsonfier>
    {
        public override Jsonfier Create() => new Jsonfier();

        public override bool Return(Jsonfier jsonfier)
        {
            jsonfier.Reset();
            return true;
        }
    }
}