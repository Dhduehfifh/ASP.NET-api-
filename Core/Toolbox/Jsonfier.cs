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