using MagicOnion;

namespace Abp.Grpc.Client.Utility
{
    /// <summary>
    /// GRpc 连接管理器
    /// </summary>
    public interface IGrpcConnectionUtility
    {
        /// <summary>
        /// 获得指定的远程服务接口
        /// </summary>
        /// <typeparam name="TService">远程服务接口类型</typeparam>
        /// <param name="serviceName">远程服务名称</param>
        TService GetRemoteService<TService>(string serviceName) where TService : IService<TService>;
    }
}
