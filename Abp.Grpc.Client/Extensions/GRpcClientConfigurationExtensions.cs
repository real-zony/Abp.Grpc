using Abp.Configuration.Startup;
using Abp.Grpc.Client.Configuration;
using Abp.Grpc.Common.Configuration;

namespace Abp.Grpc.Client.Extensions
{
    /// <summary>
    /// Grpc Client 使用时的帮助类，用于配置连接时的参数
    /// </summary>
    public static class GrpcClientConfigurationExtensions
    {
        /// <summary>
        /// 启用 Grpc Client 客户端连接
        /// </summary>
        /// <param name="configs"></param>
        /// <param name="consulConfig">Consul 服务器配置</param>
        public static void UseGrpcClientForConsul(this IModuleConfigurations configs, ConsulRegistryConfiguration consulConfig)
        {
            var clientConfig = configs.AbpConfiguration.Get<IGrpcClientConfiguration>();
            clientConfig.ConsulRegistryConfiguration = consulConfig;
        }

        /// <summary>
        /// 启用直连模式的 Grpc Client 客户端连接
        /// </summary>
        /// <param name="configs"></param>
        /// <param name="grpcNodes">Grpc 服务器节点列表</param>
        public static void UseGrpcClientForDirectConnection(this IModuleConfigurations configs, params GrpcServerNode[] grpcNodes)
        {
            var internalDict = configs.AbpConfiguration.Get<IGrpcClientConfiguration>().GrpcDirectConnectionConfiguration.GrpcServerNodes;

            foreach (var grpcNode in grpcNodes)
            {
                if (internalDict.ContainsKey(grpcNode.GrpcServiceName))
                {
                    throw new AbpInitializationException("不能添加重复的名称的 Grpc 服务节点.");
                }

                internalDict.Add(grpcNode.GrpcServiceName, grpcNode);
            }
        }
    }
}