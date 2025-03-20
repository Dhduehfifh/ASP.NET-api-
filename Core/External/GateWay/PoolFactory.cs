using System;
using Microsoft.Extensions.ObjectPool;

namespace External.Gateway
{
    public static class PoolFactory
    {
        /// 🌍 `Requester` 对象池
        private static readonly ObjectPool<Requester> _requesterPool =
            new DefaultObjectPool<Requester>(new RequesterPolicy(), 100);

        public static Requester RentRequester() => _requesterPool.Get();
        public static void ReturnRequester(Requester requester) => _requesterPool.Return(requester);

        /// 🌍 `GatewayCore` 对象池
        private static readonly ObjectPool<GatewayCore> _gatewayPool =
            new DefaultObjectPool<GatewayCore>(new GatewayCorePolicy(), 50);

        public static GatewayCore RentGateway() => _gatewayPool.Get();
        public static void ReturnGateway(GatewayCore gateway) => _gatewayPool.Return(gateway);
    }

    /// 🌍 `Requester` 对象池策略
    public class RequesterPolicy : PooledObjectPolicy<Requester>
    {
        public override Requester Create() => new Requester();
        public override bool Return(Requester requester)
        {
            requester.Reset(); // 释放资源
            return true;
        }
    }

    /// 🌍 `GatewayCore` 对象池策略
    public class GatewayCorePolicy : PooledObjectPolicy<GatewayCore>
    {
        public override GatewayCore Create() => new GatewayCore();
        public override bool Return(GatewayCore gateway)
        {
            gateway.Reset(); // 释放资源
            return true;
        }
    }
}