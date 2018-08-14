using Abp.Grpc.Common.Configuration;
using Abp.Grpc.Common.Infrastructure;
using Abp.Grpc.Server.Configuration;
using Abp.Modules;
using Consul;
using Grpc.Core;
using MagicOnion.Server;
using System;
using System.Linq;
using System.Net;

using GrpcServer = Grpc.Core.Server;

namespace Abp.Grpc.Server
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpGrpcServerModule : AbpModule
    {
        private GrpcServer _grpcServer;
        private IConsulClient _consulClient;
        private AgentServiceRegistration _agentServiceRegistration;

        private const string ServiceName = "Grpc";

        public override void PreInitialize()
        {
            IocManager.Register<IConsulClientFactory, ConsulClientFactory>();
            IocManager.Register<IGrpcServerConfiguration, GrpcServerConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpGrpcServerModule).Assembly);
        }

        public override void PostInitialize()
        {
            IGrpcServerConfiguration grpcConfiguration = IocManager.Resolve<IGrpcServerConfiguration>();

            InitializeGrpcServer(grpcConfiguration);
            InitializeConsul(grpcConfiguration);
        }

        public override void Shutdown()
        {
            _grpcServer.ShutdownAsync().GetAwaiter().GetResult();
            _consulClient?.Agent.ServiceDeregister(_agentServiceRegistration.ID).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 初始化 Grpc 服务
        /// </summary>
        /// <param name="config">Grpc 配置项</param>
        private void InitializeGrpcServer(IGrpcServerConfiguration config)
        {
            _grpcServer = new GrpcServer
            {
                Ports = { new ServerPort(config.GrpcBindAddress, config.GrpcBindPort, ServerCredentials.Insecure) },
                Services =
                {
                    MagicOnionEngine.BuildServerServiceDefinition(config.GrpcAssemblies.ToArray(),
                        new MagicOnionOptions(true))
                }
            };

            _grpcServer.Start();
        }

        /// <summary>
        /// 初始化 Consul 服务
        /// </summary>
        /// <param name="config"></param>
        private void InitializeConsul(IGrpcServerConfiguration config)
        {
            if (!config.IsEnableConsul) return;

            var currentIpAddress = GetCurrentIpAddress(config);

            _consulClient = IocManager.Resolve<IConsulClientFactory>()
                .Get(new ConsulRegistryConfiguration(config.ConsulAddress, config.ConsulPort, null));

            _agentServiceRegistration = new AgentServiceRegistration
            {
                ID = Guid.NewGuid().ToString(),
                Name = config.RegistrationServiceName,
                Address = config.GrpcBindAddress,
                Port = config.GrpcBindPort,
                Tags = new[] { ServiceName, $"urlprefix-/{config.RegistrationServiceName}" },

                // 健康检查配置
                Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(10),
                    Status = HealthStatus.Passing,
                    Timeout = TimeSpan.FromSeconds(5),
                    HTTP = $"http://{currentIpAddress}:{config.ConsulHealthCheckPort}/health/check"
                }
            };

            _consulClient.Agent.ServiceRegister(_agentServiceRegistration).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 获得当前主机的 IP 地址
        /// </summary>
        /// <exception cref="AbpInitializationException">当无法正常获得当前主机 IP 地址的时候会抛出本异常</exception>
        /// <returns>IP 地址的字符串表现形式</returns>
        private string GetCurrentIpAddress(IGrpcServerConfiguration config)
        {
            if (!string.IsNullOrEmpty(config.ConsulHealthCheckAddress)) return config.ConsulHealthCheckAddress;

            // 如果没有指定则随机分配主机地址
            IPAddress localAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();
            if (localAddress == null) throw new AbpInitializationException("无法初始化项目，无法获取到当前服务器的地址.");
            return localAddress.ToString();
        }
    }
}