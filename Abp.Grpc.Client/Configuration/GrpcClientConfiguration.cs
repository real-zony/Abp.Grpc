using Grpc.Core;
using System.Collections.Generic;

namespace Abp.Grpc.Client.Configuration
{
    /// <inheritdoc />
    public class GrpcClientConfiguration : IGrpcClientConfiguration
    {
        /// <inheritdoc />
        public Dictionary<string, IList<Channel>> GrpcServers { get; }

        /// <inheritdoc />
        public ConsulRegistryConfiguration ConsulRegistryConfiguration { get; set; }

        /// <inheritdoc />
        public GrpcClientConfiguration()
        {
            GrpcServers = new Dictionary<string, IList<Channel>>();
        }

        /// <inheritdoc />
        public IGrpcClientConfiguration AddGrpcService(string serviceName)
        {
            if (!GrpcServers.ContainsKey(serviceName))
            {
                GrpcServers.Add(serviceName, new List<Channel>());
            }

            return this;
        }

        /// <inheritdoc />
        public IGrpcClientConfiguration AddGrpcServices(IEnumerable<string> services)
        {
            foreach (var service in services)
            {
                AddGrpcService(service);
            }

            return this;
        }
    }
}