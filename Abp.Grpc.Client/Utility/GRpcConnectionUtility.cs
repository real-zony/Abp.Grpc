using Abp.Grpc.Client.Configuration;
using Abp.Grpc.Client.Infrastructure.GrpcChannel;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;

namespace Abp.Grpc.Client.Utility
{
    public class GrpcConnectionUtility : IGrpcConnectionUtility
    {
        public IGrpcClientConfiguration GrpcClientConfiguration { get; set; }
        public IGrpcChannelManager GrpcChannelManager { get; set; }

        /// <summary>
        /// 获得指定的远程服务接口
        /// </summary>
        /// <typeparam name="TService">远程服务接口类型</typeparam>
        /// <param name="serviceName">远程服务名称</param>
        public TService GetRemoteService<TService>(string serviceName) where TService : IService<TService>
        {
            var serviceChannel = GrpcChannelManager.Get(serviceName);
            if (serviceChannel == null) return default(TService);

            return MagicOnionClient.Create<TService>(serviceChannel);
        }

        /// <summary>
        /// 从模块预加载配置的固定 Grpc 服务端获取指定服务，主要用于调试使用
        /// </summary>
        /// <typeparam name="TService">远程服务接口类型</typeparam>
        /// <returns></returns>
        public TService GetRemoteServiceForDebug<TService>() where TService : IService<TService>
        {
            var newChannel = new Channel
            (
                GrpcClientConfiguration.GrpcDebugConfiguration.DebugGrpcServerIp,
                GrpcClientConfiguration.GrpcDebugConfiguration.DebugGrpcServerPort,
                ChannelCredentials.Insecure
            );

            return MagicOnionClient.Create<TService>(newChannel);
        }
    }
}