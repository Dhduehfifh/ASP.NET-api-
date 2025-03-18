using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toolbox;

namespace External
{
    public class ExternalService
    {
        private readonly ExternalGateway _gateway;

        public ExternalService(ExternalGateway gateway)
        {
            _gateway = gateway;
        }

        /// <summary>
        /// 发送 AI 任务到外部 API
        /// </summary>
        public async Task<string> SendAiTask(string aiEndpoint, Jsonfier taskData)
        {
            return await _gateway.SendJsonData(aiEndpoint, taskData);
        }

        /// <summary>
        /// 发送文件到外部存储（CDN 或 直接上传）
        /// </summary>
        public async Task<string> UploadFile(string uploadUrl, string filePath, bool useCdn = false)
        {
            return await _gateway.SendFile(uploadUrl, filePath, useCdn);
        }

        /// <summary>
        /// 查询外部 AI 任务状态
        /// </summary>
        public async Task<string> GetAiTaskStatus(string statusUrl)
        {
            return await _gateway.FetchData(statusUrl);
        }
    }
}