using Abp.Configuration.Startup;
using Abp.Grpc.Client.Configuration;
using Abp.Grpc.Common.Configuration;

namespace Abp.Grpc.Client.Extensions
{
    public static class GrpcClientConfigurationExtensions
    {
        /// <summary>
        /// 启用 Grpc Client 客户端连接
        /// </summary>
        /// <param name="configs"></param>
        /// <param name="consulConfig">Consul 服务器配置</param>
        public static void UseGrpcClient(this IModuleConfigurations configs, ConsulRegistryConfiguration consulConfig)
        {
            var clientConfig = configs.AbpConfiguration.Get<IGrpcClientConfiguration>();
            clientConfig.ConsulRegistryConfiguration = consulConfig;
        }

        /// <summary>
        /// 启用 Debug 模式的 Grpc Client 客户端连接
        /// </summary>
        /// <param name="configs"></param>
        /// <param name="grpcServerIp">调试用 Grpc 服务器的 IP</param>
        /// <param name="grpcServerPort">调试用 Grpc 服务器的 端口</param>
        public static void UseGrpcClientForDebug(this IModuleConfigurations configs, string grpcServerIp,
            int grpcServerPort)
        {
            configs.AbpConfiguration.Get<IGrpcClientConfiguration>().SetDebugMode(grpcServerIp, grpcServerPort);
        }
    }
}