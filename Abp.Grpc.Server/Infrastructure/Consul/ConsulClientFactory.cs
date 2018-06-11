using Abp.Grpc.Server.Configuration;
using Consul;
using System;

namespace Abp.Grpc.Server.Infrastructure.Consul
{
    /// <inheritdoc />
    public class ConsulClientFactory : IConsulClientFactory
    {
        /// <inheritdoc />
        public IConsulClient Get(ConsulRegistryConfiguration config)
        {
            return new ConsulClient(option =>
            {
                option.Address = new Uri($"http://{config.Host}:{config.Port}");

                if (!string.IsNullOrEmpty(config?.Token))
                {
                    option.Token = config.Token;
                }
            });
        }
    }
}