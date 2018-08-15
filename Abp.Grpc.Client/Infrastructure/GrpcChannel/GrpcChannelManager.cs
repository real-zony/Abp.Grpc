using Abp.Dependency;
using Abp.Grpc.Client.Configuration;
using Abp.Grpc.Common.Infrastructure;
using Abp.Threading;
using Abp.Threading.Extensions;
using Abp.UI;
using Grpc.Core;
using Polly;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Grpc.Client.Infrastructure.GrpcChannel
{
    public class GrpcChannelManager : IGrpcChannelManager
    {
        protected readonly Dictionary<string, Dictionary<string, Channel>> GrpcServers;

        private readonly IConsulClientFactory _consulClientFactory;
        private readonly IGrpcChannelFactory _grpcChannelFactory;
        private readonly IIocResolver _iocResolver;

        public GrpcChannelManager(IConsulClientFactory consulClientFactory,
            IGrpcChannelFactory grpcChannelFactory,
            IIocResolver iocResolver)
        {
            _consulClientFactory = consulClientFactory;
            _grpcChannelFactory = grpcChannelFactory;
            _iocResolver = iocResolver;

            GrpcServers = new Dictionary<string, Dictionary<string, Channel>>();
        }

        /// <summary>
        /// 获得一个可用的 Grpc 频道
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>可用的 Grpc 频道</returns>
        public Channel Get(string serviceName)
        {
            ReloadAvailableServices(serviceName);

            // TODO: 此处可做负载均衡策略
            if (GrpcServers[serviceName].Count <= 0) return null;

            return GrpcServers[serviceName].Values.FirstOrDefault();
        }

        /// <summary>
        /// 获得所有缓存的 Grpc 频道
        /// </summary>
        /// <returns>Grpc 频道集合</returns>
        public IReadOnlyList<Channel> GetAllChannels()
        {
            var result = new List<Channel>();

            GrpcServers.Locking(action =>
            {
                foreach (var grpcServer in GrpcServers)
                {
                    foreach (var channel in grpcServer.Value)
                    {
                        result.Add(channel.Value);
                    }
                }
            });

            return result.ToImmutableList();
        }

        /// <summary>
        /// 移除并关闭指定的频道
        /// </summary>
        /// <param name="waitCloseChannel">等待关闭的 Grpc 频道</param>
        public async Task Remove(Channel waitCloseChannel)
        {
            var queryChannel = GetAllChannels().FirstOrDefault(z => z == waitCloseChannel);

            if (queryChannel != null)
            {
                await queryChannel.ShutdownAsync();
            }
        }

        /// <summary>
        /// 重新扫描所有可用服务
        /// </summary>
        private void ReloadAvailableServices(string serviceName)
        {
            var consulConfig = _iocResolver.Resolve<IGrpcClientConfiguration>().ConsulRegistryConfiguration;

            var timeoutPolicy = Policy.Timeout(5, (context, span, arg) => throw new UserFriendlyException("Consul 集群不可用!"));

            var queryServices = new List<GrpcServerInfo>();
            timeoutPolicy.Execute(() => AsyncHelper.RunSync(async () =>
            {
                var consulClient = _consulClientFactory.Get(consulConfig);
                var services = await consulClient.Catalog.Services();

                foreach (var service in services.Response)
                {
                    var serviceInfo = await consulClient.Catalog.Service(service.Key);
                    var grpcServiceInfo = serviceInfo.Response.SkipWhile(z => !z.ServiceTags.Contains("Grpc")).Where(z => z.ServiceName == serviceName);

                    // 得到所有可用的 Grpc 服务列表
                    foreach (var grpcServer in grpcServiceInfo)
                    {
                        queryServices.Add(new GrpcServerInfo
                        {
                            ServiceId = grpcServer.ServiceID,
                            ServiceAddress = grpcServer.ServiceAddress,
                            ServicePort = grpcServer.ServicePort
                        });
                    }
                }
            }));

            // 如果不存在 Key 直接构建一个新的字典
            GrpcServers.Locking(mainDict =>
            {
                if (!GrpcServers.ContainsKey(serviceName))
                {
                    GrpcServers.Add(serviceName, new Dictionary<string, Channel>());

                    foreach (var newService in queryServices)
                    {
                        GrpcServers[serviceName].Add(newService.ServiceId, _grpcChannelFactory.Get(newService.ServiceAddress, newService.ServicePort));
                    }
                }
            });

            GrpcServers.Locking(mainDict =>
            {
                var currentServices = GrpcServers[serviceName].Keys.ToList();
                var newServices = queryServices.Select(z => z.ServiceId).ToList();

                // 已有服务与最新服务取差集，移除已经丢失的服务
                currentServices.Except(newServices).ToList().ForEach(id =>
                {
                    mainDict[serviceName][id].ShutdownAsync().Wait();
                    mainDict[serviceName].Remove(id);
                });

                // 最新服务与已有服务取差集，建立新的 Channel 通道
                newServices.Except(currentServices).ToList().ForEach(id =>
                {
                    var newGrpcServerInfo = queryServices.FirstOrDefault(z => z.ServiceId == id);
                    if (newGrpcServerInfo == null) return;

                    mainDict[serviceName].Add(id, _grpcChannelFactory.Get(newGrpcServerInfo.ServiceAddress, newGrpcServerInfo.ServicePort));
                });
            });
        }

        private class GrpcServerInfo
        {
            public string ServiceId { get; set; }

            public string ServiceAddress { get; set; }

            public int ServicePort { get; set; }
        }
    }
}