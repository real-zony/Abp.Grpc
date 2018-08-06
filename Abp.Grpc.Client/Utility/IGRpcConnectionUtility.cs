using MagicOnion;

namespace Abp.Grpc.Client.Utility
{
    /// <summary>
    /// Grpc 连接管理器
    /// </summary>
    public interface IGrpcConnectionUtility
    {
        /// <summary>
        /// 获得指定的远程服务接口
        /// </summary>
        /// <typeparam name="TService">远程服务接口类型</typeparam>
        /// <param name="serviceName">远程服务名称</param>
        TService GetRemoteService<TService>(string serviceName) where TService : IService<TService>;

        /// <summary>
        /// 从模块预加载配置的固定 Grpc 服务端获取指定服务，主要用于调试使用
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService GetRemoteServiceForDebug<TService>() where TService : IService<TService>;
    }
}