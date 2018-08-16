using Grpc.Core;
using System.Collections.Generic;

namespace Abp.Grpc.Client.Configuration
{
    /// <summary>
    /// Grpc 客户端直连配置
    /// </summary>
    public class GrpcDirectConnectionConfiguration
    {
        public Dictionary<string, GrpcServerNode> GrpcServerNodes { get; set; }

        public GrpcDirectConnectionConfiguration()
        {
            GrpcServerNodes = new Dictionary<string, GrpcServerNode>();
        }

        public GrpcServerNode this[string key]
        {
            get
            {
                if (GrpcServerNodes.TryGetValue(key, out GrpcServerNode node))
                {
                    if (string.IsNullOrEmpty(node.GrpcServiceIp)) return null;
                    if (node.GrpcServicePort <= 0 || node.GrpcServicePort > 65535) return null;

                    // 初始化 Channel 频道
                    if (node.InternalChannel == null)
                    {
                        node.InternalChannel = new Channel
                        (
                            node.GrpcServiceIp,
                            node.GrpcServicePort,
                            ChannelCredentials.Insecure
                        );
                    }

                    return node;
                }

                return null;
            }
        }
    }
}