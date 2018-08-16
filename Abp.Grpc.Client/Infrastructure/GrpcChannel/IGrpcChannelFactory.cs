using Grpc.Core;

namespace Abp.Grpc.Client.Infrastructure.GrpcChannel
{
    /// <summary>
    /// Grpc <see cref="Channel"/> 对象工厂
    /// </summary>
    public interface IGrpcChannelFactory
    {
        /// <summary>
        /// 根据提供的 Grpc 服务器地址创建一个新的 <see cref="Channel"/> 对象
        /// </summary>
        /// <param name="address">Grpc 服务器地址</param>
        /// <param name="port">Grpc 服务器端口</param>
        /// <exception cref="Abp.UI.UserFriendlyException"></exception>
        /// <returns></returns>
        Channel Get(string address, int port);
    }
}