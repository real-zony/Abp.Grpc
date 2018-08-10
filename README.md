# 0.简介

Abp.Grpc 包是基于 Abp 框架并集成 MagicOnion 实现的一个模块，能够使你的 Abp 项目支持 Grpc，并且还集成了 Consul 进行服务注册与发现。

# 1.目前存在的问题

参考 **[Issues](https://github.com/GameBelial/Abp.Grpc/issues)** 里面里程碑提列出来的问题。

# 2.包状态

| Package         |                            Status                            |
| :-------------- | :----------------------------------------------------------: |
| Abp.Grpc.Server | [![NuGet version](https://img.shields.io/badge/NuGet-3.8.2-brightgreen.svg)](https://www.nuget.org/packages/Abp.Grpc.Client/) |
| Abp.Grpc.Client | [![NuGet version](https://img.shields.io/badge/NuGet-3.8.2-brightgreen.svg)](https://www.nuget.org/packages/Abp.Grpc.Client/) |

# 3.使用方法

在定义接口的时候可能会很复杂，但是使用还是挺简单的。

## 3.1服务提供者

### 3.1.1 安装 NuGet 包

需要提供 Grpc 服务的项目，只需引用 NuGet 包 **Abp.Grpc.Client** ，之后进行相应的配置即可。

NuGet 包地址：[https://www.nuget.org/packages/Abp.Grpc.Server/](https://www.nuget.org/packages/Abp.Grpc.Server/)

包管理器安装命令：

```shell
Install-Package Abp.Grpc.Server -Version 3.8.2
```

### 3.1.2 项目模块配置

在需要使用该模块的项目的启动模块中当中添加如下引用:

```csharp
using Abp.Grpc.Server;
using Abp.Grpc.Server.Extensions;
```

之后再启动模块的类名上面注明依赖模块：

```csharp
[DependsOn(typeof(AbpGrpcServerModule)]
public class StartupModule : AbpModule
{
	// 其他代码
}
```

重载项目 Module 的 ```PreInitialize``` 方法，在当中进行如下配置:

```csharp
public override void PreInitialize()
{
    Configuration.Modules.UseGrpcService(option =>
    {
        // GRPC 服务绑定的 IP 地址
        option.GrpcBindAddress = "0.0.0.0";
        // GRPC 服务绑定的 端口号
        option.GrpcBindPort = 5001;
        // 启用 Consul 服务注册【可选】
        option.UseConsul(consulOption =>
        {
            // Consul 服务注册地址
            consulOption.ConsulAddress = "10.0.75.1";
            // Consul 服务注册端口号
            consulOption.ConsulPort = 8500;
            // 注册到 Consul 的服务名称
            consulOption.RegistrationServiceName = "TestGrpcService";
            // 健康检查接口的地址，不填为默认当前机器的随机 IP 地址
            consulOption.ConsulHealthCheckAddress = "172.31.61.41";
            // 健康检查接口的端口号
            consulOption.ConsulHealthCheckPort = 5000;
        });
    })
    .AddRpcServiceAssembly(typeof(AbpGrpcServerDemoModule).Assembly); // 扫描当前程序集的所有 GRPC 服务
}
```

### 3.1.3 健康检查配置【可选】

如果你启用了 Consul 注册，需要新建一个控制器，其名称取名为 ```HealthController``` ，并且新建一个 ```Check``` 接口，用于 Consul 进行健康检查。

```csharp
public class HealthController : HKERPControllerBase
{
	public HealthController()
	{
	}

	public IActionResult Check() => Ok("Ok");
}
```

### 3.1.4 定义提供的服务接口

如果你使用了 Abp.Grpc.Server 则需要在某个程序集当中定义你的 RPC 服务，下面代码就定义了两个简单的服务，是计算两数之和的服务。  

为了统一服务定义与调用者接口的定义，在此建议你的接口定义请单独新建一个项目，并且在该项目当中引用 Abp.Grpc.Common 库，用于定义服务提供者所提供的 Grpc 接口。

```csharp
public interface IMyService : IService<IMyService>
{
    UnaryResult<int> CalculateNumber(int x,int y);
}
```

### 3.1.5 实现提供的服务接口

```csharp
public class MyService : ServiceBase<IMyService>,IMyService
{
    public UnaryResult<int> CalculateNumber(int x,int y)
    {
        return UnaryResult(x + y);
    }
}
```

实现了服务接口之后就只需要在客户端引入 Abp.Grpc.Client 配置好服务即可进行使用。

### 3.1.6 注意事项

在定义 Rpc 方法的时候，如果需要序列化自定义对象的话，需要在该对象类型定义上方打上 ```[MessagePackObject(true)]``` 标签，例如：

```csharp
[MessagePackObject(true)]
public class UserIdentifierDto
{

   public int? TenantId { get; set; }

	public long UserId { get; set; }
}
```

然后我们的接口定义如下：

```csharp
public interface IUserAuthenticationService : IService<IUserAuthenticationService>, ITransientDependency
{
	/// <summary>
	/// 检测用户是否存在，存在的话则返回对应的用户数据
	/// </summary>
	/// <returns>验证结果，验证成功的话返回用户相应的数据</returns>
	UnaryResult<UserVerificationResultModel> UserIsExist(UserVerificationModel user);
}
```

## 3.2 服务调用者

### 3.2.1 安装 NuGet 包

NuGet 包地址：[https://www.nuget.org/packages/Abp.Grpc.Client/](https://www.nuget.org/packages/Abp.Grpc.Client/)

包管理器安装命令：

```shell
Install-Package Abp.Grpc.Client -Version 3.8.2
```

### 3.2.2 项目模块配置

首先在 Web.Core 项目或者 Web.Host 的模块的 ```PreInitiailze()``` 方法当中进行配置，首先引入以下命名空间：

```csharp
using Abp.GRpc.Client;
using Abp.Grpc.Client.Extensions;
using Abp.Grpc.Common.Configuration;
```

然后在启动模块的顶部加上依赖特性：

```csharp
[DependsOn(typeof(AbpGrpcClientModule)]
public class StartupModule : AbpModule
{
	// 其他代码
}
```

重载其 ```PreInitialize``` 方法，进行模块配置：

```csharp
public override void PreInitialize()
{
    // 传入 Consul 的 IP 地址与端口号
	Configuration.Modules.UseGrpcClient(new ConsulRegistryConfiguration("10.0.75.1", 8500, null));
}
```

### 3.2.3 引用服务提供者的接口

在这里添加服务提供者所发布 Grpc 接口的 NuGet 包，或者直接引用该项目，以便调用接口。在 DEMO 项目中即是引用的项目，当然为了版本管理，你可以将该项目发布为 NuGet 包，客户端在使用的时候需要引用该 NuGet 包。

### 3.2.4 调用接口

要调用远程服务，需要在你是用的地方注入连接管理器 ```IGrpcConnectionUtility```，然后通过 ```IGrpcConnectionUtility.GetRemoteService()``` 方法即可获得服务并进行调用。

```csharp
public class TestAppService : ApplicationService
{
	private readonly IGrpcConnectionUtility _connectionUtility;
	
	public TestAppService(IGrpcConnectionUtility connectionUtility)
    {
    	_connectionUtility = connectionUtility;
    }
    
    public async Task TestMethod()
    {
    	// 从 Consul 当中获取 BasicDataServer 服务提供者集群，然后获取到 IMyService 的接口
    	var service = await _connectionUtility.GetRemoteService<IMyService>("BasicDataServer");
    	// 调用服务 IMyService 服务提供的 CalculateNumber 接口
    	var result = await service.CalculateNumber(1,1);
    	Console.WriteLine("计算结果:" + result);
    	return Task.FromResult(0);
    }
}
```

## 4.DEMO 地址

如果你仍然对以上说明感觉到困惑，请参考 DEMO 进行实践。

服务端 DEMO：[https://github.com/GameBelial/Abp.Grpc.Server.Demo](https://github.com/GameBelial/Abp.Grpc.Server.Demo)

客户端 DEMO：[https://github.com/GameBelial/Abp.Grpc.Client.Demo](https://github.com/GameBelial/Abp.Grpc.Client.Demo)
