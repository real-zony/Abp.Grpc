namespace Abp.Grpc.Server.Configuration
{
    /// <summary>
    /// Consul 注册配置
    /// </summary>
    public class ConsulRegistryConfiguration
    {
        /// <summary>
        /// Consul 注册配置
        /// </summary>
        /// <param name="host">Consul Client 代理地址</param>
        /// <param name="port">Consul 服务注册端口</param>
        /// <param name="token">Consul Token</param>
        public ConsulRegistryConfiguration(string host, int port, string token)
        {
            Host = string.IsNullOrEmpty(host) ? "localhost" : host;
            Port = port > 0 ? port : 8500;
            Token = token;
        }

        /// <summary>
        /// Consul Client 代理地址
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Consul 服务注册端口
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Consul Token
        /// </summary>
        public string Token { get; }
    }
}