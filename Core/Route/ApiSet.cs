using System;
using System.Collections.Generic;
using External.Gateway;
using Core.Database;

namespace Routes
{
    public class ApiSet
    {
        private readonly string _basePath;
        private readonly List<ApiIndividual> _apiIndividuals = new();
        private DatabaseCore? _database;  // 🔹【Config 相关】可绑定 `数据库`
        private GatewayCore? _gateway;  // 🔹【Config 相关】可绑定 `API 网关`

        public ApiSet(string basePath)
        {
            _basePath = basePath;
        }

        public ApiSet BindApi(ApiIndividual api)
        {
            _apiIndividuals.Add(api);
            return this;
        }

        public ApiSet BindDb(DatabaseCore database)  // 🔹【Config 相关】可绑定 `数据库`
        {
            _database = database;
            return this;
        }

        public ApiSet BindGateway(GatewayCore gateway)  // 🔹【Config 相关】可绑定 `外部 API`
        {
            _gateway = gateway;
            return this;
        }

        public string ExecuteApi(string path, string requestData)
        {
            foreach (var api in _apiIndividuals)
            {
                if (api.GetPath() == path)
                {
                    return api.Execute(requestData);
                }
            }
            return "❌ [ApiSet] API 路径未找到！";
        }
    }
}