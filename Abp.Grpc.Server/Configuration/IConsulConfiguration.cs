namespace Abp.Grpc.Server.Configuration
{
    /// <summary>
    /// Consul 配置
    /// </summary>
    public interface IConsulConfiguration
    {
        /// <summary>
        /// 是否开启了 Consul 集成
        /// </summary>
        bool IsEnableConsul { get; set; }

        /// <summary>
        /// Consul 注册地址
        /// </summary>
        string ConsulAddress { get; set; }

        /// <summary>
        /// Consul 注册端口
        /// </summary>
        int ConsulPort { get; set; }

        /// <summary>
        /// 服务健康检查接口的端口
        /// </summary>
        int ConsulHealthCheckPort { get; set; }
        
        /// <summary>
        /// 服务健康检查接口的地址
        /// </summary>
        string ConsulHealthCheckAddress { get; set; }

        /// <summary>
        /// 注册到 Consul 的服务名称
        /// </summary>
        string RegistrationServiceName { get; set; }
    }
}