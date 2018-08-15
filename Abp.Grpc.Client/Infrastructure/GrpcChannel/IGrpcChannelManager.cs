using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Grpc.Client.Infrastructure.GrpcChannel
{
    public interface IGrpcChannelManager
    {
        /// <summary>
        /// 获得一个可用的 Grpc 频道
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>可用的 Grpc 频道</returns>
        Channel Get(string serviceName);

        /// <summary>
        /// 获得所有缓存的 Grpc 频道
        /// </summary>
        /// <returns>Grpc 频道集合</returns>
        IReadOnlyList<Channel> GetAllChannels();

        /// <summary>
        /// 移除并关闭指定的频道
        /// </summary>
        /// <param name="waitCloseChannel">等待关闭的 Grpc 频道</param>
        Task Remove(Channel waitCloseChannel);
    }
}