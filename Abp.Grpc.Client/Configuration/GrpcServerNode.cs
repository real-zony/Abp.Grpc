using Grpc.Core;

namespace Abp.Grpc.Client.Configuration
{
    /// <summary>
    /// Grpc 服务提供者节点
    /// </summary>
    public class GrpcServerNode
    {
        /// <summary>
        /// 服务名称，该字段为一个唯一标识，用于标识具体的 Grpc 服务节点
        /// </summary>
        public string GrpcServiceName { get; set; }

        /// <summary>
        /// Grpc 服务提供者的 IP 地址，一般为负载均衡器的地址
        /// </summary>
        public string GrpcServiceIp { get; set; }

        /// <summary>
        /// Grpc 服务提供者的 Port 端口，一般为负载均衡器的端口
        /// </summary>
        public int GrpcServicePort { get; set; }

        /// <summary>
        /// 内部的 Grpc 频道
        /// </summary>
        public Channel InternalChannel { get; set; }
    }
}