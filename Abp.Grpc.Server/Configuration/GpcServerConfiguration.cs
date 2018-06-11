using System.Collections.Generic;
using System.Reflection;

namespace Abp.Grpc.Server.Configuration
{
    /// <inheritdoc />
    public class GpcServerConfiguration : IGrpcServerConfiguration
    {
        private readonly List<Assembly> _grpcAssemblies;

        /// <inheritdoc />
        public GpcServerConfiguration()
        {
            IsEnableConsul = false;
            _grpcAssemblies = new List<Assembly>();
        }

        /// <inheritdoc />
        public bool IsEnableConsul { get; set; }

        /// <inheritdoc />
        public string ConsulAddress { get; set; }

        /// <inheritdoc />
        public int ConsulPort { get; set; }

        /// <inheritdoc />
        public int ConsulHealthCheckPort { get; set; }

        /// <inheritdoc />
        public string RegistrationServiceName { get; set; }

        /// <inheritdoc />
        public string GrpcBindAddress { get; set; }

        /// <inheritdoc />
        public int GrpcBindPort { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<Assembly> GrpcAssemblies => _grpcAssemblies;

        /// <inheritdoc />
        public void AddRpcServiceAssembly(Assembly serviceAssembly)
        {
            _grpcAssemblies.Add(serviceAssembly);
        }
    }
}