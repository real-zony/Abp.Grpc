using System;
using System.Linq;
using System.Net;
using System.Threading;
using Abp.Dependency;
using Abp.Grpc.Common.Configuration;
using Abp.Grpc.Common.Infrastructure;
using Abp.Grpc.Server.Configuration;
using Abp.Grpc.Server.DependencyInject;
using Abp.Modules;
using Abp.Threading;
using Castle.MicroKernel.Registration;
using Consul;
using Grpc.Core;
using MagicOnion.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GrpcServer = Grpc.Core.Server;

namespace Abp.Grpc.Server
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpGrpcServerModule : AbpModule
    {
        private IConsulClient _consulClient;
        private AgentServiceRegistration _agentServiceRegistration;

        private const string GrpcServiceTag = "Grpc";

        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IConsulClientFactory, ConsulClientFactory>();
            IocManager.Register<IGrpcServerConfiguration, GrpcServerConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpGrpcServerModule).Assembly);

            // TODO: 此处为了防止在 Grpc 服务器启动前 Client 开始调用服务，所以放在 Initialize 进行初始化服务器
            IGrpcServerConfiguration grpcConfiguration = IocManager.Resolve<IGrpcServerConfiguration>();
            InitializeGrpcServer(grpcConfiguration);

            if (grpcConfiguration.IsEnableConsul)
            {
                InitializeConsul(grpcConfiguration);
            }
        }

        public override void Shutdown()
        {
            AsyncHelper.RunSync(() => IocManager.Resolve<IHostedService>().StopAsync(CancellationToken.None));
            _consulClient?.Agent.ServiceDeregister(_agentServiceRegistration.ID).Wait();
        }

        /// <summary>
        /// 初始化 Grpc 服务
        /// </summary>
        /// <param name="config">Grpc 配置项</param>
        private void InitializeGrpcServer(IGrpcServerConfiguration config)
        {
            var options = new MagicOnionOptions();
            options.IsReturnExceptionStackTraceInErrorDetail = true;
            
            var serviceLocator = new CasteWindsorServiceLocatorBridge(IocManager);

            options.ServiceLocator = serviceLocator;

            // 构建 gRpc 服务。
            MagicOnionServiceDefinition serviceDefine = null;

            if (config.GrpcAssemblies != null)
            {
                serviceDefine = MagicOnionEngine.BuildServerServiceDefinition(config.GrpcAssemblies.ToArray(), options);
            }
            
            // 注入 gRpc 服务到 IoC 容器当中。
            IocManager.IocContainer.Register(Component.For<MagicOnionServiceDefinition>().Instance(serviceDefine).LifestyleSingleton());
            IocManager.IocContainer.Register(Component.For<IHostedService>().Instance(new PackageMagicOnionServerService(serviceDefine, new[]
            {
                new ServerPort(config.GrpcBindAddress, config.GrpcBindPort, ServerCredentials.Insecure)
            }, null)).LifestyleSingleton());

            AsyncHelper.RunSync(()=>IocManager.Resolve<IHostedService>().StartAsync(CancellationToken.None));
        }

        /// <summary>
        /// 初始化 Consul 服务
        /// </summary>
        /// <param name="config"></param>
        private void InitializeConsul(IGrpcServerConfiguration config)
        {
            var currentIpAddress = GetCurrentIpAddress(config);

            _consulClient = IocManager.Resolve<IConsulClientFactory>().Get(new ConsulRegistryConfiguration(config.ConsulAddress, config.ConsulPort, null));

            _agentServiceRegistration = new AgentServiceRegistration
            {
                ID = Guid.NewGuid().ToString(),
                Name = config.RegistrationServiceName,
                Address = config.ConsulAddress,
                Port = config.ConsulPort,
                Tags = new[] { GrpcServiceTag, $"urlprefix-/{config.RegistrationServiceName}" },

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
