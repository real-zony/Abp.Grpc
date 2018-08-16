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
        GrpcDirectConnectionConfiguration GrpcDirectConnectionConfiguration { get; set; }
    }
}