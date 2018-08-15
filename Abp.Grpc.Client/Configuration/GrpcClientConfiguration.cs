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
        /// Debug 模式的 Grpc 服务端配置
        /// </summary>
        public GrpcDebugConfiguration GrpcDebugConfiguration { get; set; }

        /// <summary>
        /// 是否处于调试模式
        /// </summary>
        public bool IsDebugMode { get; private set; }

        /// <summary>
        /// 启用调试模式
        /// </summary>
        /// <param name="debugGrpcServerIp">远程 Grpc 服务器 IP</param>
        /// <param name="debugGrpcServerPort">远程 Grpc 服务器端口</param>
        public void EnableDebugMode(string debugGrpcServerIp, int debugGrpcServerPort)
        {
            GrpcDebugConfiguration = new GrpcDebugConfiguration
            {
                DebugGrpcServerIp = debugGrpcServerIp,
                DebugGrpcServerPort = debugGrpcServerPort
            };

            IsDebugMode = GrpcDebugConfiguration != null;
        }
    }
}