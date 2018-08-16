using Abp.Grpc.Client.Configuration;
using Abp.Grpc.Client.Infrastructure.GrpcChannel;
using MagicOnion;
using MagicOnion.Client;

namespace Abp.Grpc.Client.Utility
{
    public class GrpcConnectionUtility : IGrpcConnectionUtility
    {
        public IGrpcClientConfiguration GrpcClientConfiguration { get; set; }
        public IGrpcChannelManager GrpcChannelManager { get; set; }

        /// <summary>
        /// 直接从 Consul 当中根据服务名称取得可用服务，但是负载均衡需要由用户实现
        /// </summary>
        /// <typeparam name="TService">远程服务接口类型</typeparam>
        /// <param name="serviceName">远程服务名称</param>
        public TService GetRemoteServiceForConsul<TService>(string serviceName) where TService : IService<TService>
        {
            var serviceChannel = GrpcChannelManager.Get(serviceName);
            if (serviceChannel == null) return default(TService);

            return MagicOnionClient.Create<TService>(serviceChannel);
        }

        /// <summary>
        /// 使用缓存的指定服务 Channel 访问 Grpc 接口，出现异常时会抛出错误信息
        /// </summary>
        /// <typeparam name="TService">远程接口服务类型</typeparam>
        /// <param name="serviceName">远程服务名称</param>
        public TService GetRemoteServiceForDirectConnection<TService>(string serviceName) where TService : IService<TService>
        {
            var nodeInfo = GrpcClientConfiguration.GrpcDirectConnectionConfiguration[serviceName];

            if (nodeInfo == null) throw new AbpException("Grpc 服务没有在启动时定义，或者初始化内部 Channel 失败.");

            if (nodeInfo.InternalChannel != null)
            {
                return MagicOnionClient.Create<TService>(nodeInfo.InternalChannel);
            }

            throw new AbpException("无法创建 Grpc 服务.");
        }
    }
}