using Abp.Grpc.Client.Configuration;
using Abp.UI;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using System.Linq;

namespace Abp.Grpc.Client.Utility
{
    /// <inheritdoc />
    public class GrpcConnectionUtility : IGrpcConnectionUtility
    {
        private readonly IGrpcClientConfiguration _clientConfiguration;

        /// <inheritdoc />
        public GrpcConnectionUtility(IGrpcClientConfiguration clientConfiguration)
        {
            _clientConfiguration = clientConfiguration;
        }

        /// <inheritdoc />
        public TService GetRemoteService<TService>(string serviceName) where TService : IService<TService>
        {
            if (!_clientConfiguration.GrpcServers.ContainsKey(serviceName)) throw new UserFriendlyException("你所选择的服务不存在，无法获取服务.");

            // TODO: 此处的节点应该做负载均衡和可用检测
            var serviceChannel = _clientConfiguration.GrpcServers[serviceName].FirstOrDefault();
            if (serviceChannel == null) throw new UserFriendlyException("对应的服务下面没有可用的服务节点。");

            return MagicOnionClient.Create<TService>(serviceChannel);
        }

        /// <inheritdoc />
        public TService GetRemoteServiceForDebug<TService>() where TService : IService<TService>
        {
            return MagicOnionClient.Create<TService>(new Channel(_clientConfiguration.DebugGrpcServerIp,
                _clientConfiguration.DebugGrpcServerPort, ChannelCredentials.Insecure));
        }
    }
}