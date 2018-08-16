using Abp.Grpc.Common.Configuration;

namespace Abp.Grpc.Client.Configuration
{
    /// <summary>
    /// GRpc 客户端默认配置
    /// </summary>
    internal class GrpcClientConfiguration : IGrpcClientConfiguration
    {
        /// <summary>
        /// Consul 注册配置
        /// </summary>
        public ConsulRegistryConfiguration ConsulRegistryConfiguration { get; set; }

        /// <summary>
        /// 直连模式需要进行的配置
        /// </summary>
        public GrpcDirectConnectionConfiguration GrpcDirectConnectionConfiguration { get; set; }

        public GrpcClientConfiguration()
        {
            GrpcDirectConnectionConfiguration = new GrpcDirectConnectionConfiguration();
        }
    }
}