# 0.简介

Abp.Grpc 包是基于 Abp 框架并集成 MagicOnion 实现的一个模块，能够使你的 Abp 项目支持 Grpc，并且还集成了 Consul 进行服务注册与发现。  

![流程图](https://blog.myzony.com/content/images/2018/08/TIM--20180815122602.png)

# 1.目前存在的问题

参考 **[Issues](https://github.com/GameBelial/Abp.Grpc/issues)** 里面里程碑提列出来的问题。

# 2.包状态

| Package         |                            Status                            |
| :-------------- | :----------------------------------------------------------: |
| Abp.Grpc.Server | [![NuGet version](https://img.shields.io/badge/nuget-4.3.0-brightgreen.svg)](https://www.nuget.org/packages/Abp.Grpc.Server/) |
| Abp.Grpc.Client | [![NuGet version](https://img.shields.io/badge/nuget-4.3.0-brightgreen.svg)](https://www.nuget.org/packages/Abp.Grpc.Client/) |

# 3.使用方法

在定义接口的时候可能会很复杂，但是使用还是挺简单的。

## 3.1 Grpc 服务提供者

### 3.1.1 安装 NuGet 包

需要提供 Grpc 服务的项目，只需引用 NuGet 包 **Abp.Grpc.Client** ，之后进行相应的配置即可。

NuGet 包地址：[https://www.nuget.org/packages/Abp.Grpc.Server/](https://www.nuget.org/packages/Abp.Grpc.Server/)

包管理器安装命令：

```shell
Install-Package Abp.Grpc.Server -Version 3.9.0.1
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
    })
    .AddRpcServiceAssembly(typeof(AbpGrpcServerDemoModule).Assembly); // 扫描当前程序集的所有 GRPC 服务
}
```

#### 3.1.2.1 【可选】启用 Consul 注册

如果需要启用 Consul 注册的话，则在 Module 的 ```PreInitialize``` 方法当中使用 ```UseConsul()``` 方法配置 Consul 服务器，代码如下。

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

## 3.2 Grpc 服务调用者

### 3.2.1 安装 NuGet 包

NuGet 包地址：[https://www.nuget.org/packages/Abp.Grpc.Client/](https://www.nuget.org/packages/Abp.Grpc.Client/)

包管理器安装命令：

```shell
Install-Package Abp.Grpc.Client -Version 3.9.0.1
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

#### 3.2.2.1 Consul 发现模式

重载其 ```PreInitialize``` 方法，进行模块配置：

```csharp
public override void PreInitialize()
{
    // 如果你需要客户端进行负载均衡操作，请传入 Consul 的 IP 地址与端口号
    Configuration.Modules.UseGrpcClient(new ConsulRegistryConfiguration("10.0.75.1", 8500, null));
}
```

#### 3.2.2.2 直连模式

重载其 ```PreInitialize``` 方法，进行模块配置：

```csharp
public override void PreInitialize()
{
	// 使用直连模式
	Configuration.Modules.UseGrpcClientForDirectConnection(new[]
	{
		new GrpcServerNode
		{
			GrpcServiceIp = "127.0.0.1",
			GrpcServiceName = "TestServiceName",
			GrpcServicePort = 40001
		}
	});   
}
```

### 3.2.3 引用服务提供者的接口

在这里添加服务提供者所发布 Grpc 接口的 NuGet 包，或者直接引用该项目，以便调用接口。在 DEMO 项目中即是引用的项目，当然为了版本管理，你可以将该项目发布为 NuGet 包，客户端在使用的时候需要引用该 NuGet 包。

### 3.2.4 调用接口

要调用远程服务，需要在你是用的地方注入连接管理器 ```IGrpcConnectionUtility```，然后通过 ```IGrpcConnectionUtility.GetRemoteService()``` 方法即可获得服务并进行调用。

当然 Consul 模式与直连模式获取服务的方法都是不一样的，下面代码展示了两者之间的区别。

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

## 3.3 使用直连模式还是 Consul 模式？

使用直连模式，那么如果要实现负载均衡的话，十分方便，直接在 GrpcNode 节点定义填写时，填上负载均衡器的地址即可。

使用 Consul 模式，则需要用户自己重写 ```IGrpcChannelManager``` 实现，自己实现负载均衡算法。

> **在这里我建议直接使用直连模式。**

# 4. Session 状态传递

因为本模块是基于 Abp 框架的，所以在我们开发一个 Grpc 接口的时候，A 平台调用 B 平台提供的服务时，需要传递用户状态。而我们可以通过在接口定义时附加一个 ```GrpcSession``` 参数，通过该参数我们可以传递 A 平台调用时他的 ```AbpSession``` 的值。

我们可以定义一个接口，该接口主要打印当前登录用户的 ```UserId``` 信息：

```csharp
/// <summary>
/// 打印调用者传递的用户 ID 数据，并返回其转换为字符串的结果
/// </summary>
/// <param name="session">AbpSession 的值</param>
/// <returns>用户 ID</returns>
UnaryResult<string> PrintCurrentUserId(GrpcSession session);
```

在该接口内部，可以通过 ```IAbpSession``` 提供的 ```Use()``` 方法临时将当前服务端平台登录用户的 ```UserId``` 进行临时变更。

```csharp
public UnaryResult<string> PrintCurrentUserId(GrpcSession session)
{
    Console.WriteLine($"接收客户端传递 Session 值之前，服务端的用户 Id 值: {_tmpAbpSession.UserId}");
    string resultUserIdStr;

    using (_tmpAbpSession.Use(session.TenantId, session.UserId))
    {
        resultUserIdStr = (_tmpAbpSession.UserId ?? 0).ToString();
        Console.WriteLine($"临时变更的 AbpSession 值: {_tmpAbpSession.UserId}");
    }

    Console.WriteLine($"退出 using 语句块时，当前用户的 Id 值: {_tmpAbpSession.UserId}");

    return new UnaryResult<string>(resultUserIdStr);
}
```

那么在客户端调用的时候，只需要传递客户端当前的 ```IAbpSession``` 值即可。

```csharp
public void TestMethod()
{
	// ... 其他代码
	// 取得 IAbpSession 对象
	var abpSession = bootstarp.IocManager.Resolve<IAbpSession>();
	// 临时更改
	using (abpSession.Use(1000,2000))
	{
		var userId = services.PrintCurrentUserId(abpSession as AbpSessionBase).GetAwaiter().GetResult();
		Console.WriteLine($"服务端收到的 UserId 值: {userId}");
	}
	// ... 其他代码
}
```

# 5. DEMO 项目

如果你仍然针对上述说明存有疑问，那么可以跳转到 [DEMO](http://) 目录下，运行 DEMO 项目进行了解。
