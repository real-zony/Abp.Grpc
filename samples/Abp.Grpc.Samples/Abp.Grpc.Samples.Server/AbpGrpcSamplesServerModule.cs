using Abp.AspNetCore;
using Abp.Grpc.Server;
using Abp.Grpc.Server.Extensions;
using Abp.Modules;

namespace Abp.Grpc.Samples.Server
{
    [DependsOn(typeof(AbpAspNetCoreModule),
        typeof(AbpGrpcServerModule))]
    public class AbpGrpcSamplesServerModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.UseGrpcService(option =>
            {
                option.GrpcBindAddress = "0.0.0.0";
                option.GrpcBindPort = 40001;
            }).AddRpcServiceAssembly(typeof(AbpGrpcSamplesServerModule).Assembly);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpGrpcSamplesServerModule).Assembly);
        }
    }
}