using System.Collections.Generic;
using System.Reflection;

namespace Abp.Grpc.Server.Configuration
{
    /// <summary>
    /// Grpc 服务端配置
    /// </summary>
    public interface IGrpcServerConfiguration : IConsulConfiguration, IGrpcConfiguration
    {
        /// <summary>
        /// 存在 Grpc 服务的程序集集合
        /// </summary>
        IReadOnlyList<Assembly> GrpcAssemblies { get; }

        /// <summary>
        /// 添加包含 Grpc 服务定义的程序集
        /// </summary>
        /// <param name="serviceAssembly">服务程序集</param>
        void AddRpcServiceAssembly(Assembly serviceAssembly);
    }
}