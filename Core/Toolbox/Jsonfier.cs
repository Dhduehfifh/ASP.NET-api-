using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Toolbox
{
    /// <summery>
    /// Json分块鱼对象化工具
    /// </summery>
    public class Jsonfier
    {
        //临时分块存储
        private List<ProtocolChunk> _chunks = new List<ProtocolChunk>();

        /// <summery>
        /// 添加分块（用于接受场景）
        /// </summery>
        public void AddChunk(ProtocolChunk chunk)
        {
            _chunks.Add(chunk);
        }

        /// <summery>
        /// 判断分块是否接收完成
        /// </summery>
        public bool IsComplete => _chunks.Count > 0 && _chunks.Count == _chunks[0].TotalChunks;

        /// <summery>
        /// 分块重组为完整数据
        /// </summery>
        public byte[] Reassemble()
        {
            _chunks.Sort((a, b) => a.Index.CompareTo(b.Index)); //按顺序重排
            List<byte> allData = new List<byte>();
            foreach (var chunk in _chunks)
                allData.AddRange(chunk.Data);
            return allData.ToArray(); //返回合成数据 
        }

        /// <summary>
        /// 将对象序列化为 JSON 字节数组
        /// </summary>
        public byte[] Serialize(object obj)
        {
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            return Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// 将 JSON 字节数组反序列化为对象
        /// </summary>
        public T Deserialize<T>(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// 将对象序列化后按块大小分块
        /// </summary>
        public List<ProtocolChunk> Chunk(object obj, int chunkSize)
        {
            var data = Serialize(obj); // 序列化成字节
            List<ProtocolChunk> chunks = new List<ProtocolChunk>();
            int totalChunks = (int)Math.Ceiling((double)data.Length / chunkSize); // 总块数

            for (int i = 0; i < totalChunks; i++)
            {
                int offset = i * chunkSize;
                int size = Math.Min(chunkSize, data.Length - offset);
                byte[] chunkData = new byte[size];
                Array.Copy(data, offset, chunkData, 0, size);

                chunks.Add(new ProtocolChunk
                {
                    Index = i,
                    TotalChunks = totalChunks,
                    Data = chunkData
                });
            }

            return chunks;
        }

    }

    /// <summery>
    /// 分块协议格式，指出序号，总块数，数据
    /// </summery>
    public class ProtocolChunk
    {
        public int Index { get; set; }
        public int TotalChunks { get; set; }
        public byte[] Data { get; set; }
    }
}