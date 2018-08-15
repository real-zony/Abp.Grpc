using Abp.Grpc.Common.Configuration;

namespace Abp.Grpc.Client.Configuration
{
    /// <summary>
    /// Grpc 客户端配置
    /// </summary>
    public interface IGrpcClientConfiguration
    {
        /// <summary>
        /// Consul 注册配置
        /// </summary>
        ConsulRegistryConfiguration ConsulRegistryConfiguration { get; set; }

        /// <summary>
        /// Debug 模式的 Grpc 服务端配置
        /// </summary>
        GrpcDebugConfiguration GrpcDebugConfiguration { get; set; }

        /// <summary>
        /// 启用调试模式
        /// </summary>
        /// <param name="debugGrpcServerIp">远程 Grpc 服务器 IP</param>
        /// <param name="debugGrpcServerPort">远程 Grpc 服务器端口</param>
        void EnableDebugMode(string debugGrpcServerIp, int debugGrpcServerPort);
    }
}