using Abp.Configuration.Startup;
using Abp.Grpc.Client.Configuration;

namespace Abp.Grpc.Client.Extensions
{
    public static class GrpcClientConfigurationExtensions
    {
        /// <summary>
        /// 启用 Grpc Client 客户端连接
        /// </summary>
        /// <param name="consulConfig">Consul 服务器配置</param>
        public static IGrpcClientConfiguration UseGrpcClient(this IModuleConfigurations configs, ConsulRegistryConfiguration consulConfig)
        {
            var clientConfig = configs.AbpConfiguration.Get<IGrpcClientConfiguration>();
            clientConfig.ConsulRegistryConfiguration = consulConfig;
            return clientConfig;
        }
    }
}
