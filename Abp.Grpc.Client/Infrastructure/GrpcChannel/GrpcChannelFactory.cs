using Abp.UI;
using Grpc.Core;
using System;

namespace Abp.Grpc.Client.Infrastructure.GrpcChannel
{
    /// <inheritdoc />
    public class GrpcChannelFactory : IGrpcChannelFactory
    {
        /// <inheritdoc />
        public Channel Get(string address, int port)
        {
            try
            {
                return new Channel(address, port, ChannelCredentials.Insecure);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Grpc 频道创建失败，详细信息请查看内部异常.", e);
            }
        }
    }
}