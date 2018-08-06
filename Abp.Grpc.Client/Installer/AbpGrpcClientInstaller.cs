using Abp.Grpc.Client.Configuration;
using Abp.Grpc.Client.Infrastructure.Consul;
using Abp.Grpc.Client.Infrastructure.GrpcChannel;
using Abp.Grpc.Client.Utility;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Grpc.Client.Installer
{
    public class AbpGrpcClientInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IConsulClientFactory, ConsulClientFactory>().ImplementedBy<ConsulClientFactory>().LifestyleSingleton(),
                Component.For<IGrpcClientConfiguration, GrpcClientConfiguration>().ImplementedBy<GrpcClientConfiguration>().LifestyleSingleton(),
                Component.For<IGrpcChannelFactory, GrpcChannelFactory>().ImplementedBy<GrpcChannelFactory>().LifestyleSingleton(),
                Component.For<IGrpcConnectionUtility, GrpcConnectionUtility>().ImplementedBy<GrpcConnectionUtility>().LifestyleSingleton()
                );
        }
    }
}