using MagicOnion;

namespace Abp.Grpc.Client.Utility
{
    /// <summary>
    /// Grpc 连接管理器
    /// </summary>
    public interface IGrpcConnectionUtility
    {
        /// <summary>
        /// 直接从 Consul 当中根据服务名称取得可用服务，但是负载均衡需要由用户实现
        /// </summary>
        /// <typeparam name="TService">远程服务接口类型</typeparam>
        /// <param name="serviceName">远程服务名称</param>
        TService GetRemoteServiceForConsul<TService>(string serviceName) where TService : IService<TService>;

        /// <summary>
        /// 使用缓存的指定服务 Channel 访问 Grpc 接口，出现异常时会抛出错误信息
        /// </summary>
        /// <typeparam name="TService">远程接口服务类型</typeparam>
        /// <param name="serviceName">远程服务名称</param>
        /// <returns></returns>
        TService GetRemoteServiceForDirectConnection<TService>(string serviceName) where TService : IService<TService>;
    }
}