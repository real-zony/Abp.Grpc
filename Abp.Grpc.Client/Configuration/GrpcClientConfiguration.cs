using Abp.Grpc.Common.Configuration;
using Grpc.Core;
using System.Collections.Generic;

namespace Abp.Grpc.Client.Configuration
{
    /// <summary>
    /// GRpc 客户端默认配置
    /// </summary>
    public class GrpcClientConfiguration : IGrpcClientConfiguration
    {
        /// <summary>
        /// 已经注册完成的服务 IP/Port 集合
        /// </summary>
        public Dictionary<string, IList<Channel>> GrpcServers { get; }

        /// <summary>
        /// Consul 注册配置
        /// </summary>
        public ConsulRegistryConfiguration ConsulRegistryConfiguration { get; set; }

        /// <summary>
        /// GRpc 客户端默认配置
        /// </summary>
        public GrpcClientConfiguration()
        {
            GrpcServers = new Dictionary<string, IList<Channel>>();
        }

        /// <summary>
        /// 添加新的 Grpc Service 提供者
        /// </summary>
        /// <param name="serviceName">Grpc 服务名称</param>
        public IGrpcClientConfiguration AddGrpcService(string serviceName)
        {
            if (!GrpcServers.ContainsKey(serviceName))
            {
                GrpcServers.Add(serviceName, new List<Channel>());
            }

            return this;
        }

        /// <summary>
        /// 添加新的 Grpc Service 提供者
        /// </summary>
        /// <param name="services">Grpc 服务名称集合</param>
        /// <returns></returns>
        public IGrpcClientConfiguration AddGrpcServices(IEnumerable<string> services)
        {
            foreach (var service in services)
            {
                AddGrpcService(service);
            }

            return this;
        }

        /// <summary>
        /// 启用调试模式
        /// </summary>
        /// <param name="debugGrpcServerIp">远程 Grpc 服务器 IP</param>
        /// <param name="debugGrpcServerPort">远程 Grpc 服务器端口</param>
        public void SetDebugMode(string debugGrpcServerIp, int debugGrpcServerPort)
        {
            IsDebugMode = true;
            DebugGrpcServerIp = debugGrpcServerIp;
            DebugGrpcServerPort = debugGrpcServerPort;
        }

        /// <summary>
        /// 是否处于调试模式
        /// </summary>
        public bool IsDebugMode { get; private set; }

        /// <summary>
        /// 调试模式时，指定的远程 Grpc 提供服务器 IP
        /// </summary>
        public string DebugGrpcServerIp { get; private set; }

        /// <summary>
        /// 调试模式时，指定的远程 Grpc 提供服务器端口
        /// </summary>
        public int DebugGrpcServerPort { get; private set; }
    }
}