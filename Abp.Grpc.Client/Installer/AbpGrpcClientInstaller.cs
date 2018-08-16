using Abp.Grpc.Client.Configuration;
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
                Component.For<IGrpcClientConfiguration, GrpcClientConfiguration>().ImplementedBy<GrpcClientConfiguration>().LifestyleSingleton()
                );
        }
    }
}