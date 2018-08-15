using Abp.Dependency;
using Abp.Grpc.Client.Configuration;
using Abp.Grpc.Client.Infrastructure.GrpcChannel;
using Abp.Grpc.Client.Installer;
using Abp.Modules;

namespace Abp.Grpc.Client
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpGrpcClientModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.IocContainer.Install(new AbpGrpcClientInstaller());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpGrpcClientModule).Assembly,
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });
        }

        public override void Shutdown()
        {
            var configuration = IocManager.Resolve<GrpcClientConfiguration>();

            // 未处于调试模式则需要清空所有连接
            if (!configuration.IsDebugMode)
            {
                var channelManager = IocManager.Resolve<IGrpcChannelManager>();
                foreach (var channel in channelManager.GetAllChannels())
                {
                    channelManager.Remove(channel).GetAwaiter().GetResult();
                }
            }
        }
    }
}