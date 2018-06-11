using Grpc.Core;

namespace Abp.Grpc.Client.Infrastructure.GrpcChannel
{
    /// <summary>
    /// GRpc <see cref="Channel"/> 对象工厂
    /// </summary>
    public interface IGrpcChannelFactory
    {
        /// <summary>
        /// 根据提供的 GRpc 服务器地址创建一个新的 <see cref="Channel"/> 对象
        /// </summary>
        /// <param name="address">GRpc 服务器地址</param>
        /// <param name="port">GRpc 服务器端口</param>
        /// <exception cref="Abp.UI.UserFriendlyException"></exception>
        /// <returns></returns>
        Channel Get(string address, int port);
    }
}