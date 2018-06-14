# Abp.Grpc.Server 使用说明
## 简介

本模块使网站项目支持 Grpc 服务，并且能够向 Consul 注册自己，以便使客户端能够调用本站点提供的 Grpc 服务。

## 安装

NUGET 包地址：[https://www.nuget.org/packages/Abp.Grpc.Server/](https://www.nuget.org/packages/Abp.Grpc.Server/)
包名称：Abp.Grpc.Server
包管理器命令：

```shell
Install-Package Abp.Grpc.Server -Version 1.0.1
```

包版本：1.0.1

## 配置

### 模块配置

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
    // 启用 Grpc 服务
	Configuration.Modules.UseGrpcService(option =>
	{
        // 配置 Grpc 绑定的 IP 地址，一般默认为 0.0.0.0
		option.GrpcBindAddress = "0.0.0.0";
        // 配置 Grpc 绑定的开放端口
		option.GrpcBindPort = 5001;
        // 启用 Consul 服务注册，可选，但我们开发的时候统一必选
		option.UseConsul(consulOption =>
		{
            // Consul 注册地址
			consulOption.ConsulAddress = "192.168.100.107";
			// Consul 注册的端口
            consulOption.ConsulPort = 8500;
            // 当前 Grpc 注册的服务名称
			consulOption.RegistrationServiceName = "BasicDataServer";
            // 健康检查端口
			consulOption.ConsulHealthCheckPort = 5000;
		});
	}).AddRpcServiceAssembly(typeof(HKERPGrpcServiceModule).Assembly);
}
```

### 健康检查配置

配置完成模块之后，下面新建一个控制器，其名称取名为 ```HealthController``` ，并且新建一个 ```Check``` 接口，用于 Consul 进行健康检查。

```csharp
public class HealthController : HKERPControllerBase
{
	public HealthController()
	{
	}

	public IActionResult Check() => Ok("Ok");
}
```

## 使用

如果你使用了 Abp.Grpc.Server 则需要在某个程序集当中定义你的 RPC 服务，下面代码就定义了两个简单的服务，是计算两数之和的服务。  

### 定义服务接口

```csharp
public interface IMyService : IService<IMyService>
{
    UnaryResult<int> CalculateNumber(int x,int y);
}
```

### 实现服务接口

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

## 注意事项

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

# Abp.Grpc.Client 使用说明
## 简介

本模块使网站/程序能够支持 Grpc 服务调用，从 Consul 当中发现可用的 Rpc 服务，并且进行调用。

## 安装

NUGET 包地址：[https://www.nuget.org/packages/Abp.Grpc.Client/](https://www.nuget.org/packages/Abp.Grpc.Client/)
包名称：Abp.Grpc.Client
包管理器命令：

```shell
Install-Package Abp.Grpc.Client -Version 1.0.7	
```

包版本：1.0.7

## 配置

首先在 Web.Core 项目或者 Web.Host 的模块的 ```PreInitiailze()``` 方法当中进行配置，首先引入以下命名空间：

```csharp
using Abp.GRpc.Client;
using Abp.GRpc.Client.Configuration;
using Abp.GRpc.Client.Extensions;
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
	Configuration.Modules.UseGrpcClient(new ConsulRegistryConfiguration("192.168.100.107", 8500, null));
}
```

## 使用

### 定义服务接口

要使用远程的 RPC 服务就需要你拥有跟远程服务一样的接口定义，最佳实践应该是将定义独立为一个项目，Server 与 Client 引用同一个项目即可，下面我们来定义 RPC 服务的接口：

```csharp
public interface IMyService : IService<IMyService>
{
    UnaryResult<int> CalculateNumber(int x,int y);
}
```

这里的定义与 Server 那边一样，所以我们定义好之后就可以调用了。

### 注入连接管理器

要调用远程服务，需要在你是用的地方注入连接管理器 ```IGrpcConnectionUtility```，然后通过 ```IGrpcConnectionUtility.GetRemoteService``` 方法即可获得服务。

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
    	var result = await _connectionUtility.GetRemoteService<IMyService>("BasicDataServer").CalculateNumber(1,1);
    	Console.WriteLine("计算结果:" + result);
    	return Task.FromResult(0);
    }
}
```

这里注意你的服务名称一定要与之前注册的服务名称一致。

## 注意事项

这里的服务名称应该是存放在 appsettings 里面，通过Configuration 来获取具体的服务名称，此处是为了演示而直接硬编码书写，实际开发过程中，服务名称应该是由 appsettings 里面拿去，其节点应该统一定义为：

```json
{
  "GrpcServer": {
    // 基础数据微服务
    "BasicDataServerName": "BasicDataGrpc"
  }
}
```

