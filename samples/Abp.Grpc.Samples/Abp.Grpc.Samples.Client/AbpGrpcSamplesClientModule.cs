using Abp.Grpc.Client;
using Abp.Grpc.Client.Configuration;
using Abp.Grpc.Client.Extensions;
using Abp.Grpc.Common.Configuration;
using Abp.Modules;

namespace Abp.Grpc.Samples.Client
{
    [DependsOn(typeof(AbpGrpcClientModule))]
    public class AbpGrpcSamplesClientModule : AbpModule
    {
        public override void PreInitialize()
        {
            // 直连模式
            Configuration.Modules.UseGrpcClientForDirectConnection(new[]
            {
                new GrpcServerNode
                {
                    GrpcServiceIp = "127.0.0.1",
                    GrpcServiceName = "TestServiceName",
                    GrpcServicePort = 40001
                }
            });
            
            // Consul 发现模式
            Configuration.Modules.UseGrpcClientForConsul(new ConsulRegistryConfiguration("Consul 服务器IP",40000,"Token 值"));
        }
    }
}