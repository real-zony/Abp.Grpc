using Grpc.Core;
using System.Collections.Generic;

namespace Abp.Grpc.Client.Configuration
{
    /// <summary>
    /// GRpc 客户端配置
    /// </summary>
    public interface IGrpcClientConfiguration
    {
        /// <summary>
        /// 已经注册完成的服务 IP/Port 集合
        /// </summary>
        Dictionary<string, IList<Channel>> GrpcServers { get; }

        /// <summary>
        /// Consul 注册配置
        /// </summary>
        ConsulRegistryConfiguration ConsulRegistryConfiguration { get; set; }

        /// <summary>
        /// 添加新的 Grpc Service 提供者
        /// </summary>
        /// <param name="serviceName">Grpc 服务名称</param>
        IGrpcClientConfiguration AddGrpcService(string serviceName);

        /// <summary>
        /// 添加新的 Grpc Service 提供者
        /// </summary>
        /// <param name="services">Grpc 服务名称集合</param>
        /// <returns></returns>
        IGrpcClientConfiguration AddGrpcServices(IEnumerable<string> services);
    }
}
