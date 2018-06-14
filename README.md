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

