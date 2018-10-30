using System;
using Abp.Grpc.Client.Utility;
using Abp.Grpc.Samples.Client.GrpcService;
using Abp.Runtime.Session;

namespace Abp.Grpc.Samples.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bootstarp = AbpBootstrapper.Create<AbpGrpcSamplesClientModule>())
            {
                bootstarp.Initialize();
                
                // 解析 Grpc 链接管理器
                var connectionUtility = bootstarp.IocManager.Resolve<IGrpcConnectionUtility>();
                
                // 通过直连的方式 Grpc 服务，这里的 TestServiceName 是在模块预加载时定义的名称
                var services = connectionUtility.GetRemoteServiceForDirectConnection<ITestService>("TestServiceName");
                
                // 通过 Consul 获取 Grpc 服务，而这里的 TestServiceName 则是服务注册到 Consul 时的服务名称
                // var services = connectionUtility.GetRemoteServiceForConsul<ITestService>("TestServiceName");

                
                // 调用计算两数之和的接口，并打印结果
                // 这里的 Sum 是一个异步方法，可以通过 async/await 关键字简化调用方式
                var result = services.Sum(1, 1).GetAwaiter().GetResult();
                Console.WriteLine($"调用 Sum 方法的结果：{result}");
                
                // 传递 AbpSession 值并且打印服务端取得的 UserId 值
                var abpSession = bootstarp.IocManager.Resolve<IAbpSession>();
                using (abpSession.Use(1000,2000))
                {
                    var userId = services.PrintCurrentUserId(abpSession as AbpSessionBase).GetAwaiter().GetResult();
                    Console.WriteLine($"服务端收到的 UserId 值: {userId}");
                }

                Console.ReadLine();
            }
        }
    }
}